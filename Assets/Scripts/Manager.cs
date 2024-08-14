using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class Manager : MonoBehaviour
{

	public VerticalLayoutGroup buttonGroup;
	public HorizontalLayoutGroup bottomRow;
	public RectTransform canvasRect;
	public GameObject DialogHolder;
	public GameObject DialogBody;
	[SerializeField] GameObject QuitBtn;

	public TMP_Text[] OptionTexts;
	CalcButton[] bottomButtons;
	public Button[] OptionButtons;

	public Text digitLabel;
	public Text operatorLabel;
	public TMP_Text BrainLabel;
	bool errorDisplayed;
	bool displayValid;
	bool specialAction;
	double currentVal;
	double storedVal;
	double result;
	char storedOperator;

	bool canvasChanged;

	int _brain=100;

	public static Manager Instance;


	private void Awake()
	{
		bottomButtons = bottomRow.GetComponentsInChildren<CalcButton>();
		Instance = this;
	}


	// Use this for initialization
	void Start()
	{
		_brain = 100;
		bottomRow.childControlWidth = false;
		canvasChanged = true;
		buttonTapped('c');
    }

	// Update is called once per frame
	void Update()
	{
		if (canvasChanged)
		{
			canvasChanged = false;
			adjustButtons();
		}

	}

	private void OnRectTransformDimensionsChange()
	{
		canvasChanged = true;
	}

	void adjustButtons()
	{
		if (bottomButtons == null || bottomButtons.Length == 0)
			return;
		float buttonSize = canvasRect.sizeDelta.x / 4;
		float bWidth = buttonSize - bottomRow.spacing;
		for (int i = 1; i < bottomButtons.Length; i++)
		{
			bottomButtons[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																	 bWidth);
		}
		bottomButtons[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
																 bWidth * 2 + bottomRow.spacing);
	}

	void clearCalc()
	{
		digitLabel.text = "0";
		operatorLabel.text = "";
		specialAction = displayValid = errorDisplayed = false;
		currentVal = result = storedVal = 0;
		storedOperator = ' ';
	}
	void updateDigitLabel()
	{
		if (!errorDisplayed)
			digitLabel.text = currentVal.ToString();
		displayValid = false;
	}

	void calcResult(char activeOp)
	{
		switch (activeOp)
		{
			case '=':
				{
					result = currentVal;
					
				}
				break;
			case '+':
				result = storedVal + currentVal;
				break;
			case '-':
				result = storedVal - currentVal;
				break;
			case 'x':
				result = storedVal * currentVal;
				break;
			case '÷':
				if (currentVal != 0)
				{
					result = storedVal / currentVal;
				}
				else
				{
					errorDisplayed = true;
					digitLabel.text = "ERROR";
				}
				break;
			default:
				Debug.Log("unknown: " + activeOp);
				break;
		}
		currentVal = result;
		updateDigitLabel();
	}

	public void buttonTapped(char caption)
	{
		if (errorDisplayed)
			clearCalc();

		if ((caption >= '0' && caption <= '9') || caption == '.')
		{
			if (digitLabel.text.Length < 15 || !displayValid)
			{
				if (!displayValid)
					digitLabel.text = (caption == '.' ? "0" : "");
				else if (digitLabel.text == "0" && caption != '.')
					digitLabel.text = "";
				digitLabel.text += caption;
				displayValid = true;
			}
		}
		else if (caption == 'c')
		{
			clearCalc();
		}
		else if (caption == '±')
		{
			currentVal = -double.Parse(digitLabel.text);
			updateDigitLabel();
			specialAction = true;
		}
		else if (caption == '%')
		{
			currentVal = double.Parse(digitLabel.text) / 100.0d;
			updateDigitLabel();
			specialAction = true;
		}
		else if (displayValid || storedOperator == '=' || specialAction)
		{
			currentVal = double.Parse(digitLabel.text);
			displayValid = false;
			if (storedOperator != ' ')
			{
				calcResult(storedOperator);
				storedOperator = ' ';
				if (caption == '=')
				{
                    DialogHolder.SetActive(true);
					DialogHolder.GetComponent<Animator>().SetTrigger("FadeIn");
                    RefreshDialogBox();
                }
                
            }
			operatorLabel.text = caption.ToString();
			storedOperator = caption;
			storedVal = currentVal;
			updateDigitLabel();
			specialAction = false;
		}
	}
	
	void RefreshDialogBox()
	{
		BrainLabel.text = "Brain : "+_brain.ToString();
		foreach (var btn in OptionButtons)
		{
			btn.GetComponent<Image>().color= Color.white;
			if(_brain>0)
				btn.interactable = true;
		}
		var crtIndex = Random.Range(0, 3);
		for (int i = 0; i < OptionTexts.Length; i++) {
			if (i == crtIndex)
				OptionTexts[i].text = digitLabel.text.ToString();
			else
				OptionTexts[i].text = (currentVal + (int)Random.Range((float)-currentVal-1, (float)currentVal+1)).ToString();
		}
	}
	
	

	public bool CheckCorrectAnswer(string valueText)
	{
		bool isCorrect=false;
		foreach(var btn in OptionButtons)
		{
			btn.interactable= false;
		}
		

        if (valueText == digitLabel.text.ToString())
		{
			isCorrect = true;
		}
		else
		{
			if (_brain > 0)
			{
				_brain -= 20;
				BrainLabel.text = "Brain : " + _brain.ToString();
			}
			if(_brain<=0)
				BrainLabel.text = "Do not use Calculator, your brain is af!";

            isCorrect = false;
		}
		
		if(_brain>0)
		{
            DialogHolder.GetComponent<Animator>().SetTrigger("Fade");

        }
		else
		{
			DialogBody.SetActive(false);
			QuitBtn.SetActive(true);
		}


        return isCorrect;
    }

    public IEnumerator DisableGOAfterDelay()
	{
		if (_brain > 0)
		{
			yield return new WaitForSeconds(3f);
			DialogHolder?.SetActive(false);
		}
		else
			yield return null;
	}

	public void QuitApp()
	{
		Application.Quit();
	}

}
