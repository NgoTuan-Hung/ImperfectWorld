using System.Collections.Generic;
using UnityEngine.EventSystems;

public partial class GameUIManager
{
    public void ChangeGameInteractionButtonBattle() => gameInteractionButtonTMP.text = "Battle !";

    public void ChangeGameInteractionButtonShop() => gameInteractionButtonTMP.text = "Finish !";

    public enum GuideDialogState
    {
        State1,
        State2,
    }

    public GuideDialogState guideDialogState = GuideDialogState.State1;
    public InteractiveButtonUI guideBribeButton;

    public void HandleGuideDialog()
    {
        switch (guideDialogState)
        {
            case GuideDialogState.State1:
            {
                guideDialogBox.Show();
                guideDialogState = GuideDialogState.State2;
                break;
            }
            case GuideDialogState.State2:
            {
                guideDialogBox.Hide();
                guideDialogState = GuideDialogState.State1;
                break;
            }
            default:
                break;
        }
    }

    public void GuideBribeSuccess()
    {
        guideDialogBox.Hide();
    }
}
