using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChooseActionsPanelScript : MonoBehaviour {

    public GameManager gameManager;

    public bool active = true;

    public ActionButtonScript actionButton1;
    public ActionButtonScript actionButton2;
    public Button sendButton;
    // Use this for initialization

    public void EnableButtons(bool gele)
    {
        active = true;
        actionButton1.button.interactable = true;
        sendButton.interactable = true;

        if (gele)
        {
            actionButton2.setAction(0);
            //actionButton2.Freeze();
            actionButton2.button.interactable = false;
        }
        else
        {
            actionButton2.button.interactable = true;
            actionButton2.Unfreeze();
        }
    }

    public void DisableButtons()
    {
        sendButton.interactable = false;
        active = false;
        actionButton1.button.interactable = false;
        actionButton1.ClosePanel();
        actionButton2.button.interactable = false;
        actionButton2.ClosePanel();
    }

    public void SendActions()
    {
        if (active)
        {
            gameManager.ActionsSet((int)actionButton1.actualAction, (int)actionButton2.actualAction);
            //client.Send("13;" + (actionButton1.actualAction - 1) + ";" + (actionButton1.actualAction - 1));
        }
    }

    public void SetActionsEnabled(bool allowpousse, bool allowregard, bool allowcontrole, bool allowdeplace)
    {
        actionButton1.enableAction(1, allowpousse); actionButton2.enableAction(1, allowpousse);
        actionButton1.enableAction(2, allowregard); actionButton2.enableAction(2, allowregard);
        actionButton1.enableAction(3, allowcontrole); actionButton2.enableAction(3, allowcontrole);
        actionButton1.enableAction(4, allowdeplace); actionButton2.enableAction(4, allowdeplace);
    }
}
