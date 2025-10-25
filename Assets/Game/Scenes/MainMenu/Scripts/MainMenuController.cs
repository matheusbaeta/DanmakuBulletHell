using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform buttonParent;
    [SerializeField] private Button levelButton;
    [SerializeField] private List<LevelData> levelDataList;

    private void Awake()
    {
        foreach (var levelData in levelDataList)
        {
            var button = Instantiate(levelButton, buttonParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = levelData.levelName;
            button.onClick.AddListener(() => {
                GameplayController.Show(new GameplayData() { levelData = levelData });
            });
        }
    }
}
