using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text stageText;

    [SerializeField]
    private Button button;

    private int stageIndex;

    private StageSelectPopupUI stageSelectUI;

    private bool unlocked;

    public void Init( int index, StageSelectPopupUI selectUI)
    {
        stageIndex = index;
        stageSelectUI = selectUI;
        Unlock();

        stageText.text = (index + 1).ToString();

        button.onClick.AddListener(OnClick);

    }

    public void OnClick()
    {
        stageSelectUI.SelectStage(stageIndex);
    }

    public void Unlock()
    {
        unlocked = stageIndex <= SaveManager.CurrentStage;
        button.interactable = unlocked;
    }
}