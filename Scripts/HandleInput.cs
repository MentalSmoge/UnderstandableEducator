using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;







/*
 * ����������, ��� ����� �������� ��������
 * 
 * ��������� ��� ����� ������ 
 * ���� ��� ������ - �������
 * ���� ���� �� ������� - ��������
 * 
 * ������� �� ��������� ������� � ��������� ���� ������
 * 
 * ����� �������� � ���������
 * 
 * ������������ �����
 * 
 * �����
 * 
 * ����
 * 
 * 
*/
public class HandleInput : MonoBehaviour
{
    ParentCommand currentCommand;
    public GameObject robot;
    public TMP_InputField InputFieldText;
    public List<Button> temp_buttons;
    public Button buttonStop;
    public Button buttonPlay;
    //public Button buttonNext;
    public Button buttonMenu;
    public Button buttonHelp;
    public Button buttonSpravka;
    public Image imageError;
    public Image imageLose;
    public Image imageVictory;
    public Text Error;
    private Vector3 SavePosition;
    private Vector3 SaveDirection;
    private Vector3 SaveRotation;
    public bool isPlaying = false;
    public List<GameObject> buttons;
    public bool ErrorDoNotShow = false;

    private void Start()
    {
        temp_buttons = FindObjectsOfType<Button>().ToList();
        currentCommand = new MainCommand(1);
        currentCommand.closed = true;
        SavePosition = robot.transform.position;
        SaveDirection = robot.GetComponent<PlayerMovement>().CurrentDirection;
        SaveRotation = robot.transform.eulerAngles;
    }
    void AddCommand(Command command)
    {
        currentCommand.AddCommand(command);
    }
    Command NewMove(bool forward)
	{
        return new MoveCommand(forward, 0.1f);
	}
    Command NewRotate(bool Right)
    {
        if (Right)
            return new RotateCommand(-90, 0.1f);
        else
            return new RotateCommand(90, 0.1f);
    }
    Command NewWhile(List<string> s, out bool success) //��������� �� ������ � ������ WHILE
    {
        string left = "";
        Words words = FromStringToWords(s, out left);
        success = true;

        if (words.BaseDecryptions[0].GetDecryption() == Decryption.Wall) //�����
		{
            if (words.BaseDecryptions[1].GetDecryption() == Decryption.InFront) //�������
			{
                return new WhileCommand(new Condition_WallIsInDirection(true, true), currentCommand);
			}
            else if (words.BaseDecryptions[1].GetDecryption() == Decryption.Behind) //������
            {
                return new WhileCommand(new Condition_WallIsInDirection(false, true), currentCommand);
            }
        }
        if (words.BaseDecryptions[0].GetDecryption() == Decryption.Not) //��
        {
            if (words.BaseDecryptions[1].GetDecryption() == Decryption.Wall) // �����
            {
                if (words.BaseDecryptions[2].GetDecryption() == Decryption.InFront)//�������
                {
                    return new WhileCommand(new Condition_WallIsInDirection(true, false), currentCommand);
                }
                else if (words.BaseDecryptions[2].GetDecryption() == Decryption.Behind) //������
                {
                    return new WhileCommand(new Condition_WallIsInDirection(false, false), currentCommand);
                }
            }
        }
        //������ �� ������ - ������ �����.
        success = false;
        return new WhileCommand(new Condition_WallIsInDirection(true, false), currentCommand);
    }
    Command NewFor(List<string> s, out bool success, int line)
    {
        Words words = FromStringToNumber(s, out success, line);
        success = true;
        return new ForCommand(words.BaseDecryptions[0].content, currentCommand);
    }
    Command NewIf(List<string> s) //���� �� ������������
    {
        return new IfCommand(new Condition_WallIsInDirection(true, false), currentCommand);
    }
    

    public Words FromStringToWords(List<string> s, out string what_is_left)
	{

        var localization = new Dictionary<string, Decryption>()
        {
            ["wall"] = Decryption.Wall,
            ["�����"] = Decryption.Wall,
            ["�����"] = Decryption.Wall,

            ["not"] = Decryption.Not,
            ["��"] = Decryption.Not,
            ["���"] = Decryption.Not,

            ["infront"] = Decryption.InFront,
            ["�������"] = Decryption.InFront,

            ["behind"] = Decryption.Behind,
            ["�����"] = Decryption.Behind,
        };

        string temp ="";
        Words words = new Words();

        for (int i = 0; i < s.Count; i++)
		{
            temp += s[i].ToLower();
            if (localization.ContainsKey(temp))
            {
                words.Add(new Word(localization[temp])); //����� ����� - ��������� � WORDS
                temp = "";
            }
            else
			{
                int result;
                if (int.TryParse(temp, out result) == true)
                {
                    words.Add(new Number(result)); //����� ����� - ��������� � WORDS
                    temp = "";    
                }
			}

        }
        what_is_left = temp;
        return words;
	}
    public Words FromStringToNumber(List<string> s, out bool success, int line)
    {
        Words words = new Words();

        success = true;
        if (s.Count > 1)
		{
            success = false;
        }
        int result;
        if (int.TryParse(s[0], out result) == true)
        {
            if (result > 0)
			{
                words.Add(new Number(result)); //����� ����� - ��������� � WORDS
            }
            else
            {
                success = false;
            }
        }
        else
        {
            success = false;
        }
        return words;
    }

    public Action FromStringToAction(string s, out List<string> slice_without_action, out bool success,int line)
	{
        var localization = new Dictionary<string, Action>()
        {
            //������� ������
            ["rotateleft"] = Action.RotateLeft,
            ["�������������"] = Action.RotateLeft,

            //������� �������
            ["rotateright"] = Action.RotateRight,
            ["��������������"] = Action.RotateRight,
            ["�������������"] = Action.RotateRight,

            //�������� ������
            ["up"] = Action.MoveUp,
            ["������"] = Action.MoveUp,
            ["�����"] = Action.MoveUp,

            //�������� �����
            ["down"] = Action.MoveDown,
            ["�����"] = Action.MoveDown,

            //WHILE
            ["while"] = Action.While,
            ["����"] = Action.While,

            //FOR
            ["for"] = Action.For,
            ["���������"] = Action.For,

            //IF
            ["if"] = Action.If,
            ["����"] = Action.If,

            //END
            ["end"] = Action.End,
            ["�����"] = Action.End,

        };

        success = true;

        string[] sliced_s = s.Split(' ');

        slice_without_action = new List<string>();

        string temp = ""; //������� ����� ��������. � ��� ����� � �� ���������� ���� (������� ������)
        /*
         * ��������� � ���������� temp �����, �� ��� ��� ����  
         * �� ����� ������� ���� ���� �����.
         * � ������ ���� �� ����� ������� �����
         * ��������� �� ���� ������ � ��������� �� ������ � �������
         * (��� ��� �� �������)
         */
		for (int i = 0; i < sliced_s.Length; i++)
		{
            temp += sliced_s[i].ToLower();
            if (localization.ContainsKey(temp))
            {
				for (int j = i+1; j < sliced_s.Length; j++)
				{
                    if (sliced_s[j] != "")
                        slice_without_action.Add(sliced_s[j]);
				}
                return localization[temp];
            }
        }

        //� �������� ������ ���������� ������ 
        //������� ������
        success = false;
        
        return Action.MoveUp;
    }
    public void RevealError(int num, string temp)
	{
        ErrorDoNotShow = true;
        imageLose.gameObject.SetActive(false);

        BlockButtons();

        Error.text = $"������ �� ������ {num}. ������������ ������� {temp}";
        imageError.enabled = true;
        imageError.gameObject.SetActive(true);
        currentCommand.innerCommands.Clear();
        StopAllCoroutines();
        robot.transform.position = SavePosition;
        print("����� ��������� " + temp);
    }
    public void SetActivityToButton(Button button, bool active)
	{
        button.interactable = active;
    }
    public void RevealError(string temp)
    {
        print("���������");
        ErrorDoNotShow = true;
        imageLose.gameObject.SetActive(false);

        BlockButtons();

        Error.text = temp;
        imageError.enabled = true;
        imageError.gameObject.SetActive(true);
        currentCommand.innerCommands.Clear();
        StopAllCoroutines();
        robot.transform.position = SavePosition;
        print("����� ��������� " + temp);
    }
    public void RevealError(int num, List<string> temp)
    {
        ErrorDoNotShow = true;

        imageLose.gameObject.SetActive(false);

        foreach (Button item in temp_buttons)
        {
            SetActivityToButton(item, false);
        }
        string temp2 = "";
        foreach (var item in temp)
        {
            temp2 += item;
        }
        Error.text = $"������ �� ������ {num}. ������������ ������� {temp2}";
        imageError.enabled = true;
        imageError.gameObject.SetActive(true);
        currentCommand.innerCommands.Clear();
        StopAllCoroutines();
        robot.transform.position = SavePosition;
        print("����� ��������� " + temp2);
    }
    public void FromActionAndStringToCommand(Action action, List<string> slice_without_action, out bool success, int line)
    {
        Command return_command = null;
        success = true;

        switch (action)
        {
            case Action.MoveUp:
                if (slice_without_action.Count > 0) //���� � ������ ���� ���-�� ����� ����� - ������
				{
                    print("����� ������");

                    success = false;
                    return;
                }
                return_command = NewMove(true);
                currentCommand.AddCommand(return_command);
                break;
            case Action.MoveDown:
                if (slice_without_action.Count > 0) //���� � ������ ���� ���-�� ����� ������� - ������
                {
                    print("������� ������");

                    success = false;
                    return;
                }
                return_command = NewMove(false);
                currentCommand.AddCommand(return_command);
                break;
            case Action.RotateRight:
                if (slice_without_action.Count > 0) //���� � ������ ���� ���-�� ����� ���������� - ������
                {
                    print("���������� ������");

                    success = false;
                    return;
                }
                return_command = NewRotate(true);
                currentCommand.AddCommand(return_command);
                break;
            case Action.RotateLeft:
                if (slice_without_action.Count > 0) //���� � ������ ���� ���-�� ����� ���������� - ������
                {
                    print("���������� ������");

                    success = false;
                    return;
                }
                return_command = NewRotate(false);
                currentCommand.AddCommand(return_command);
                break;
            case Action.While:
                string left = "";
                Words words = FromStringToWords(slice_without_action, out left);
                if (left != "")
                {
                    success = false;
                    return;
                }
                if (slice_without_action.Count == 0)
				{
                    success = false;
                    return;
                }
                bool canBe = false;
                if (words.BaseDecryptions[0].GetDecryption() == Decryption.Wall) //�����
                {
                    if (words.BaseDecryptions.Count > 1)
					{
                        if (words.BaseDecryptions[1].GetDecryption() == Decryption.InFront) //�������
                        {
                            canBe = true;
                        }
                        else if (words.BaseDecryptions[1].GetDecryption() == Decryption.Behind) //������
                        {
                            canBe = true;

                        }
                    }
                }
                if (words.BaseDecryptions[0].GetDecryption() == Decryption.Not) //��
                {
                    if (words.BaseDecryptions.Count > 1)
					{
                        if (words.BaseDecryptions[1].GetDecryption() == Decryption.Wall) // �����
                        {
                            if (words.BaseDecryptions.Count > 2)
							{
                                if (words.BaseDecryptions[2].GetDecryption() == Decryption.InFront)//�������
                                {
                                    canBe = true;
                                }
                                else if (words.BaseDecryptions[2].GetDecryption() == Decryption.Behind) //������
                                {
                                    canBe = true;
                                }
                            }
                        }
                    }
                }
                print(canBe);
                if (!canBe)
				{
					success = false;
                    return;
				}
                return_command = NewWhile(slice_without_action, out success);
                print(success);
                currentCommand.AddCommand(return_command);
                currentCommand = (ParentCommand)return_command;
                break;
            case Action.For:
                bool number = true;
                if (slice_without_action == null)
				{
                    success = false;
                    return;
                }

                if (slice_without_action.Count == 0)
                {
                    success = false;
                    return;
                }
                Words words1 = FromStringToNumber(slice_without_action, out number, line);
                if (!number)
				{
                    success = false;
                    return;
                }
                return_command = NewFor(slice_without_action, out success, line);
                currentCommand.AddCommand(return_command);
                currentCommand = (ParentCommand)return_command;
                break;
            case Action.End:
                if (currentCommand.closed == true)
				{
                    success = false;
                    return;
                }
                currentCommand.closed = true;
                ParentCommand temp2 = currentCommand.parent; //������ �������������� currentCommand � �������� �������� currentCommand
                currentCommand = temp2;
                break;
            default:
                break;
        }
        //print(return_command);
    }
    public bool FromListToCommands(List<string> commands)
	{
        int line = 0;
		for (int i = 0; i < commands.Count; i++)
		{
            line = i + 1;
            List<string> slice_without_action;
            bool success;
            Action action = FromStringToAction(commands[i], out slice_without_action, out success,line);
            if (!success)
            {
                RevealError(line, slice_without_action);
                print("����� �� ������ �� ����� " + line);
                return false;
			}
            FromActionAndStringToCommand(action, slice_without_action, out success, line);
            if (!success)
			{
                RevealError(line, slice_without_action);
                print("����� ��������� ����� �� ����� " + line);
                return false;
            }
        }
        return true;
	}
    public List<string> FromTextToList(string commands)
    {
        string[] vs1 = commands.Split('\n');
        List<string> vs = vs1.ToList();


        vs.RemoveAll(s => string.IsNullOrWhiteSpace(s));

        return vs;
    }
    public void BlockButtons()
	{
        foreach (Button item in temp_buttons)
        {
            SetActivityToButton(item, false);
        }
    }
    public void CheckVictory()
    {
        BlockButtons();
        foreach (var item in buttons)
        {
            if (item.GetComponent<TileButton>().Pressed == false)
			{
                if (!ErrorDoNotShow)
                    imageLose.gameObject.SetActive(true);
                return;
			}
        }
        imageVictory.gameObject.SetActive(true);
    }
    public void RestoreLevel()
    {
        ErrorDoNotShow = false;

        SetActivityToButton(buttonPlay, true);
        SetActivityToButton(buttonStop, false);
        //SetActivityToButton(buttonNext, true);
        SetActivityToButton(buttonHelp, true);
        SetActivityToButton(buttonMenu, true);
        SetActivityToButton(buttonSpravka, true);
        StopAllCoroutines();
        foreach (var item in buttons)
        {
            item.GetComponent<TileButton>().unPress();
        }
        robot.transform.position = SavePosition;
        robot.transform.eulerAngles = SaveRotation;
        robot.GetComponent<PlayerMovement>().CurrentDirection = SaveDirection;
        isPlaying = false;
    }
    public void ButtonPressed(string button_name)
	{
		switch (button_name)
		{
            case "Stop":
                break;

            case "Next":
                break;
            case "Start":
                if (!isPlaying) //������ �� ���� ������
				{
					isPlaying = true;  //��������� � ����� ������������
                    SetActivityToButton(buttonPlay, false);
                    //SetActivityToButton(buttonNext, false);
                    SetActivityToButton(buttonStop, true);
                    List<string> vs = FromTextToList(InputFieldText.text.Trim()); //�������� ������ �� \n

                    currentCommand = new MainCommand(1); //����������� ������� �������
                    currentCommand.closed = true;

                    bool success = FromListToCommands(vs); //��������� �� ����� � ��������

                    if (success)
					{
                        currentCommand.CheckExecute(); //�������� ������ �� �������� ���� ������

                        StartCoroutine(currentCommand.Execute(robot)); //�������� ���
                    }
                }
                break;
            default:
				break;
		}
	}
    public class Words
	{
        public Action Action;
        public List<BaseDecryption> BaseDecryptions = new List<BaseDecryption>();

        public void Add(BaseDecryption baseDecryption)
		{
            BaseDecryptions.Add(baseDecryption);
		}
    }
    public class Word : BaseDecryption
    {
        public Word(Decryption word)
		{
            content = (int)word;
		}
    }
    public class Number : BaseDecryption
    {
        public Number(int number)
        {
            content = number;
        }
    }
    public class BaseDecryption
    {
        public int content;
        public BaseDecryption()
        {

        }
        public Decryption GetDecryption()
        {
            return (Decryption)content;
        }
    }
    public enum Action
    {
        MoveUp,
        MoveDown,
        RotateRight,
        RotateLeft,
        While,
        For,
        If,
        End
    };

    public enum Decryption
    {
        Behind=-1,
        Wall = -2,
        Not = -3,
        InFront = -4,
    };
}
