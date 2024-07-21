using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static GameObject container;

    // ---싱글톤으로 선언--- //
    static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    // --- 게임 데이터 파일이름 설정 ("원하는 이름(영문).json") --- //
    string GameDataFileName = "GameData.json";

    // --- 저장용 클래스 변수 --- //
    public Data data = new Data();


    // 불러오기
    public void LoadGameData()
    {
        string filePath = Application.dataPath + "/" + GameDataFileName;

        // 저장된 게임이 있다면
        if (File.Exists(filePath))
        {
            // 저장된 파일 읽어오고 Json을 클래스 형식으로 전환해서 할당
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            print("불러오기 완료");
        }
    }


    // 저장하기
    public void SaveGameData()
    {
        // 클래스를 Json 형식으로 전환 (true : 가독성 좋게 작성)
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        // 이미 저장된 파일이 있다면 덮어쓰고, 없다면 새로 만들어서 저장
        File.WriteAllText(filePath, JsonUtility.ToJson(data, true));

        //// 올바르게 저장됐는지 확인 (자유롭게 변형)
        print("저장 완료");
    }
}