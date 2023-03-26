using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static KIWI_ReadCard.Type;

namespace KIWI_ReadCard
{
    public class Reader
    {
        public struct SCARD_IO_REQUEST
        {
            public int dwProtocol;
            public int cbPciLength;
        }

        //引用 PC/SC(Personal Computer/Smart Card) API WinScard.dll
        [DllImport("WinScard.dll")]
        public static extern int SCardEstablishContext(uint dwScope,
          int nNotUsed1, int nNotUsed2, ref int phContext);
        [DllImport("WinScard.dll")] public static extern int SCardReleaseContext(int phContext);
        [DllImport("WinScard.dll")]
        public static extern int SCardConnect(int hContext, string cReaderName,
          uint dwShareMode, uint dwPrefProtocol, ref int phCard, ref int ActiveProtocol);
        [DllImport("WinScard.dll")] public static extern int SCardDisconnect(int hCard, int Disposition);
        [DllImport("WinScard.dll")]
        public static extern int SCardListReaders(int hContext, string cGroups,
          ref string cReaderLists, ref int nReaderCount);
        [DllImport("WinScard.dll")]
        public static extern int SCardTransmit(int hCard,
          ref SCARD_IO_REQUEST pioSendPci, byte[] pbSendBuffer, int cbSendLength,
          ref SCARD_IO_REQUEST pioRecvPci, ref byte pbRecvBuffer, ref int pcbRecvLength);

        public static string getData()
        {
            PatientForm patientData = new PatientForm {};
            int ContextHandle = 0, CardHandle = 0, ActiveProtocol = 0, ReaderCount = -1;
            string ReaderList = string.Empty; //讀卡機名稱列表
            SCARD_IO_REQUEST SendPci, RecvPci;
            byte[] SelectAPDU = { 0x00, 0xA4, 0x04, 0x00, 0x10, 0xD1, 0x58, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x11, 0x00 };
            byte[] ReadProfileAPDU = { 0x00, 0xca, 0x11, 0x00, 0x02, 0x00, 0x00 };
            byte[] SelectRecvBytes = new byte[2]; //應回 90 00
            int SelectRecvLength = 2;
            byte[] ProfileRecvBytes = new byte[59]; //接收Profile的 Byte Array
            int ProfileRecvLength = 59;

            //建立 Smart Card API
            if (SCardEstablishContext(0, 0, 0, ref ContextHandle) == 0)
                //列出可用的 Smart Card 讀卡機
                if (SCardListReaders(ContextHandle, null, ref ReaderList, ref ReaderCount) == 0)
                    //建立 Smart Card 連線
                    if (SCardConnect(ContextHandle, ReaderList, 1, 2, ref CardHandle, ref ActiveProtocol) == 0)
                    {
                        SendPci.dwProtocol = RecvPci.dwProtocol = ActiveProtocol;
                        SendPci.cbPciLength = RecvPci.cbPciLength = 8;
                        //下達 Select Profile 檔的 APDU
                        if (SCardTransmit(CardHandle, ref SendPci, SelectAPDU, SelectAPDU.Length,
                          ref RecvPci, ref SelectRecvBytes[0], ref SelectRecvLength) == 0)
                        //下達讀取Profile指令
                        {
                            if (SCardTransmit(CardHandle, ref SendPci, ReadProfileAPDU, ReadProfileAPDU.Length,
                              ref RecvPci, ref ProfileRecvBytes[0], ref ProfileRecvLength) == 0)
                                 Console.WriteLine(
                                      @"健保卡ID:{0}
                                      姓名:{1}
                                      身份証字號:{2}
                                      生日:{3}/{4}/{5}
                                      姓別:{6}
                                      發卡日期:{7}/{8}/{9}",
                                       Encoding.Default.GetString(ProfileRecvBytes, 0, 12),

                                       Encoding.Default.GetString(ProfileRecvBytes, 12, 6),
                                       Encoding.Default.GetString(ProfileRecvBytes, 32, 10),

                                       Encoding.Default.GetString(ProfileRecvBytes, 42, 3),
                                       Encoding.Default.GetString(ProfileRecvBytes, 45, 2),
                                       Encoding.Default.GetString(ProfileRecvBytes, 47, 2),

                                       Encoding.Default.GetString(ProfileRecvBytes, 49, 1),

                                       Encoding.Default.GetString(ProfileRecvBytes, 51, 2),
                                       Encoding.Default.GetString(ProfileRecvBytes, 53, 2),
                                       Encoding.Default.GetString(ProfileRecvBytes, 55, 2)
                                   );
                                string Year = (Int16.Parse(Encoding.Default.GetString(ProfileRecvBytes, 42, 3)) + 1911).ToString();
                                string Month = Encoding.Default.GetString(ProfileRecvBytes, 45, 2);
                                string Date = Encoding.Default.GetString(ProfileRecvBytes, 47, 2);
                                patientData.name = Encoding.Default.GetString(ProfileRecvBytes, 12, 6);
                                patientData.id = Encoding.Default.GetString(ProfileRecvBytes, 32, 10);
                                patientData.birth = Year + "-" + Month + "-" + Date;
                                patientData.gender = Encoding.Default.GetString(ProfileRecvBytes, 49, 1).ToLower();
                        }
                    }
            return JsonConvert.SerializeObject(patientData);
        }
    }
}
