using UnityEngine;
using UnityEngine.UI;
using Gridly;

public class ShowProcessDownloadToScreen : MonoBehaviour
{
    [SerializeField] private SyncDataGridly m_SyncComponent;
    [SerializeField] private Text m_ProgressPercent;
    [SerializeField] private Text m_ProgressAmount;
    [SerializeField] private GameObject m_DownloadingIndicator;
    [SerializeField] private Image m_ProgressFillImage;

    private bool isFinish;
    
    public void Update()
    {
        m_ProgressAmount.text = $"{SyncDataGridly.ProgressDone} / {SyncDataGridly.ProgressNumberTotal}";

        m_ProgressPercent.text = isFinish
            ? "Finish"
            : $"{(int)(SyncDataGridly.Progress * 100)}%";

        m_ProgressFillImage.fillAmount = SyncDataGridly.Progress;
    }

    public void Finish()
    {
        isFinish = true;
        m_DownloadingIndicator.SetActive(false);
    }
}
