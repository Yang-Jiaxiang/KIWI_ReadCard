# KIWI__Simple_ReadCard 簡易讀卡系統

## 說明
透過WinScard.dll 智慧卡應用協議APDU 讀取健保卡"卡面"資料，其中包含 健保卡ID、姓名、身分證字號、生日、性別、補發卡日期。  
**而不是使用 衛福部 cshis.dll。**  
差別在 cshis.dll 需要經過醫事卡、健保vpn、sim碼 認證，而WinScard.dll不需要經過認證，但無法讀取細節資料。

## 系統架構
1. nodeJS api server auth 登入認證
2. WinScard.dll 智慧卡應用協議APDU 讀取健保卡"卡面"資料，其中包含 健保卡ID、姓名、身分證字號、生日、性別、補發卡日期。
3. 將patientData .post api/patient
4. .get api/worklist，獲取worklistData = ``accessionNumber、StudyInstanceUID``
5. 將 worklistData .post api/schedules & api/reports

