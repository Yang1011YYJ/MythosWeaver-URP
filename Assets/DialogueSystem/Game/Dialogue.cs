/// <summary>
/// 每一句對話者資料
/// </summary>
public class Dialogue
{
    /// <summary>
    /// Resources 位置
    /// </summary>
    private string _photoPath = "Images/CharacterPhotos";
    /// <summary>
    /// 姓名
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 大頭照
    /// </summary>
    public readonly string PhotoPath;
    /// <summary>
    /// 對話內容
    /// </summary>
    public readonly string Sentence;

    public Dialogue(string name, string sentence = "", string photoPath = null)
    {
        Name = name;
        // 可以填 null，帶入預設圖片
        photoPath = photoPath ?? "Default";
        PhotoPath = $"{_photoPath}/{photoPath}";
        Sentence = sentence;
    }
}