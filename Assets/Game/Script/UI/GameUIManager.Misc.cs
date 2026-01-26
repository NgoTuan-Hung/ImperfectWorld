using System.Collections.Generic;
using UnityEngine.EventSystems;

public partial class GameUIManager
{
    public void ChangeGameInteractionButtonBattle() => gameInteractionButtonTMP.text = "Battle !";

    public void ChangeGameInteractionButtonShop() => gameInteractionButtonTMP.text = "Finish !";

    public void ChangeGameInteractionButtonRestSite() => gameInteractionButtonTMP.text = "Finish !";

    public enum GuideDialogState
    {
        State1,
        State2,
    }

    public InteractiveButtonUI guideBribeButton;

    public void CloseGuideDialog()
    {
        guideDialogBox.Hide();
    }

    public void GuideBribeSuccess()
    {
        guideDialogBox.Hide();
    }
}
