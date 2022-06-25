using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ОСНОВНЫЕ КЛАССЫ

public class Command : MonoBehaviour
{
    public
    virtual IEnumerator Execute(GameObject robot)
    {
        yield return null;
    }
    public virtual void CheckExecute()
    {

    }
}

public class ParentCommand : Command
{
    public bool closed = false;
	public int numberOfRepetition = 1;
    public ParentCommand parent;
    public List<Command> innerCommands = new List<Command>();
    
    public void AddCommand(Command command)
    {
        innerCommands.Add(command);
    }
	public override string ToString()
	{
		return "Parent";
	}
}

public class Condition
{
    public bool IfTrueThen = true;
    public
    virtual bool Check(GameObject robot)
    {
        return false;
    }
    public override string ToString()
    {
        return "Cond";
    }
}
public class MainCommand : ParentCommand
{
    GameObject manager;
    public override IEnumerator Execute(GameObject robot)
    {
        for (int i = 0; i < numberOfRepetition; i++)
        {
            for (int j = 0; j < innerCommands.Count; j++)
            {
                yield return innerCommands[j].Execute(robot);
            }
            manager.GetComponent<HandleInput>().CheckVictory();
            //ПРОВЕРЯЕМ КНОПКИ
        }
    }
    public override void CheckExecute()
    {
        //if (innerCommands.Count == 0)
        //{
        //    manager.GetComponent<HandleInput>().RevealError("Пустая программа! Добавьте какие-либо команды");
        //    StopAllCoroutines();
        //}
        for (int i = 0; i < numberOfRepetition; i++)
        {
            for (int j = 0; j < innerCommands.Count; j++)
            {
                if (innerCommands[j] is ParentCommand)
                {
                    innerCommands[j].CheckExecute();
                }
            }
        }
    }
    public override string ToString()
    {
        return "For";
    }

    public MainCommand(int numberOfRepetition)
    {
        manager = GameObject.Find("Manager");
        this.numberOfRepetition = numberOfRepetition;
        this.parent = null;
    }
}

public class ForCommand : ParentCommand
{
    GameObject manager;

    public override IEnumerator Execute(GameObject robot)
    {
        for (int i = 0; i < numberOfRepetition; i++)
        {
            for (int j = 0; j < innerCommands.Count; j++)
            {
                yield return innerCommands[j].Execute(robot);
            }
        }
    }
    public override void CheckExecute()
    {
        if (!closed)
        {
            manager.GetComponent<HandleInput>().RevealError("Не закрыт цикл For словом end/конец!");
            StopAllCoroutines();
        }
        if (innerCommands.Count == 0)
        {
            manager.GetComponent<HandleInput>().RevealError("Пустой цикл For! Добавьте команды туда");
            StopAllCoroutines();
        }
        for (int i = 0; i < numberOfRepetition; i++)
        {
            for (int j = 0; j < innerCommands.Count; j++)
            {
                if (innerCommands[j] is ParentCommand)
				{
                    innerCommands[j].CheckExecute();
                }
            }
        }
    }
    public override string ToString()
    {
        return "For";
    }
    public ForCommand(int numberOfRepetition, ParentCommand parent)
    {
        manager = GameObject.Find("Manager");
        this.numberOfRepetition = numberOfRepetition;
        this.parent = parent;
    }
}

public class Condition_WallIsInDirection : Condition
{
    bool InFront;
    public
    override bool Check(GameObject robot)
    {
        if (InFront)
        {
            if (robot.GetComponent<PlayerMovement>().WallInFront == true)
            {
                if (IfTrueThen)
                    return true;
                else
                    return false;
            }
            else
            {
                if (IfTrueThen)
                    return false;
                else
                    return true;
            }
        }
        else
        {
            return true;
        }
    }
    public override string ToString()
    {
        return "Wall";
    }
    public Condition_WallIsInDirection(bool InFront, bool ifTrueThen)
    {
        this.IfTrueThen = ifTrueThen;
        this.InFront = InFront;
    }
}

public class Condition_WallInFront : Condition
{
    public
    override bool Check(GameObject robot)
    {
        if (robot.GetComponent<PlayerMovement>().WallInFront)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
public class Condition_WallBehind : Condition
{
    public
    override bool Check(GameObject robot)
    {
        if (robot.GetComponent<PlayerMovement>().WallInFront)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}

public class IfCommand : ParentCommand
{
    List<Condition> conditions;
    public override IEnumerator Execute(GameObject robot)
    {
        bool execute = true;
        print("WH");
        foreach (Condition item in conditions)
        {
            if (item.Check(robot) == false)
            {
                execute = false;
            }
        }
        if (execute)
        {
            for (int j = 0; j < innerCommands.Count; j++)
            {
                yield return innerCommands[j].Execute(robot);
            }
        }
    }
    public IfCommand(List<Condition> conditions, ParentCommand parent)
    {
        this.conditions = conditions;
        this.parent = parent;
        parent = this;
    }
    public IfCommand(Condition condition, ParentCommand parent)
    {
        conditions = new List<Condition>();
        conditions.Add(condition);
        this.parent = parent;
        parent = this;
    }
    public IfCommand(ParentCommand parent)
    {
        this.parent = parent;
        parent.innerCommands.Add(this);
        parent = this;
    }
}
public class WhileCommand : ParentCommand
{
    Condition condition;
    GameObject manager; 
    public override IEnumerator Execute(GameObject robot)
    {
        //print(condition.Check(robot));
        while (condition.Check(robot))
        {
            if (innerCommands.Count == 0)
            {
                manager.GetComponent<HandleInput>().RevealError("Пустой цикл While! Добавьте команды туда");
                StopAllCoroutines();
            }
            else
			{
                for (int j = 0; j < innerCommands.Count; j++)
                {
                    yield return innerCommands[j].Execute(robot);
                }
            }
        }
    }
    public override void CheckExecute()
    {
        print(innerCommands.Count);
        if (!closed)
        {
            manager.GetComponent<HandleInput>().RevealError("Не закрыт цикл While словом end/конец!");
            StopAllCoroutines();
        }
        if (innerCommands.Count == 0)
        {
            manager.GetComponent<HandleInput>().RevealError("Пустой цикл While! Добавьте команды туда");
            StopAllCoroutines();
        }
        for (int j = 0; j < innerCommands.Count; j++)
        {
            if (innerCommands[j] is ParentCommand)
            {
                innerCommands[j].CheckExecute();
            }
        }
    }
    public override string ToString()
    {
        return "While " + condition.ToString();
    }
    public WhileCommand(Condition condition, ParentCommand parent)
    {
        manager = GameObject.Find("Manager");
        this.condition = condition;
        this.parent = parent;
    }
}


public class EndCommand : ParentCommand
{
    public
    override IEnumerator Execute(GameObject robot)
    {
        yield return new WaitForSeconds(0.3f);
    }
    public EndCommand()
    {

    }
    public override string ToString()
    {
        return "End";
    }
};

public class RotateCommand : Command
{
    int angle;
    float timeToMove;
    public
    override IEnumerator Execute(GameObject robot)
    {
        yield return new WaitForSeconds(0.3f);
        yield return robot.GetComponent<PlayerMovement>().RotatePlayer(angle, timeToMove);
    }
    public RotateCommand(int angle, float timeToMove)
    {
        this.angle = angle;
        this.timeToMove = timeToMove;
    }
    public override string ToString()
    {
        return "Rotate";
    }
};

public class MoveCommand : Command
{
    GameObject manager;

    bool forward;
    float timeToMove;
    public
    override IEnumerator Execute(GameObject robot)
    {
        if (forward)
        {
            bool wall = robot.GetComponent<PlayerMovement>().WallInFront;
            if (wall)
            {
                manager.GetComponent<HandleInput>().RevealError("Робот врезался в стенку!");

                StopAllCoroutines();
                //StopCoroutine(Execute(robot));
            }
		}
        else
        {
            bool wall = robot.GetComponent<PlayerMovement>().WallBehind;
            if (wall)
            {
                manager.GetComponent<HandleInput>().RevealError("Робот врезался в стенку!");

                StopAllCoroutines();
                //StopAllCoroutines();
                //StopCoroutine(Execute(robot));
            }
        }
        yield return new WaitForSeconds(0.3f);
        yield return robot.GetComponent<PlayerMovement>().MovePlayer(forward, timeToMove);
    }
    public override string ToString()
    {
        return "Move";
    }
    public MoveCommand(bool forward, float timeToMove)
    {
        manager = GameObject.Find("Manager");

        this.forward = forward;
        this.timeToMove = timeToMove;
    }
};
