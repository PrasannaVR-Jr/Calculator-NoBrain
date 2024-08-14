using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    bool isCorrect;
    public void OnOptionClicked()
    {
        isCorrect = Manager.Instance.CheckCorrectAnswer(GetComponentInChildren<TMP_Text>().text);

        if (isCorrect)
            GetComponent<Image>().color = Color.green;
        else
            GetComponent<Image>().color = Color.red;

        StartCoroutine(Manager.Instance.DisableGOAfterDelay());
    }

}
