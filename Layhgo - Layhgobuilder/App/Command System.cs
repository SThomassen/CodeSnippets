/// <summary>
/// The command design pattern makes it possible to have a history of actions.
/// This will enable the possibility to undo and redo certain actions.
/// </summary>

public interface ICommand
{
    void Undo();
    void Redo();
}

/// <summary>
/// Base class for recording actions
/// </summary>
public abstract class Command : ICommand
{
    public abstract void Undo();
    public abstract void Redo();

    protected string m_name;
    protected int m_objectId;

    protected Vector3 m_position;
    protected Quaternion m_rotation;

    protected EObjectState m_state;

    public Command(ILayhgoObject a_target)
    {
        if (a_target == null)
            return;

        m_name = a_target.Name;
        m_position = a_target.Position;
        m_rotation = a_target.Rotation;
        m_state = a_target.GetCurrentState();
        m_objectId = a_target.GetInstanceID();
    }

    protected ILayhgoObject GetTarget()
    {
        ILayhgoObject target = GameManager.Instance.Chunk.GetObjectByInstanceID(m_objectId, m_position);
        return target;
    }
}

/// <summary>
/// Record a moved and/or rotated object
/// </summary>
public class TransformCommand : Command
{
    private Vector3 m_oldPosition;
    private Vector3 m_newPosition;
    private Quaternion m_oldRotation;
    private Quaternion m_newRotation;

    public TransformCommand(ILayhgoObject a_target, Vector3 a_initialPosition, Quaternion a_initialRotation) : base(a_target)
    {
        m_oldRotation = a_initialRotation;
        m_oldPosition = a_initialPosition;
        m_newRotation = a_target.Rotation;
        m_newPosition = a_target.Position;
    }

    public override void Redo()
    {
        m_position = m_newPosition;
        m_rotation = m_newRotation;

        ILayhgoObject target = GetTarget();
        if (target != null)
        {
            target.SetRotation(m_newRotation.eulerAngles);
            target.SetPosition(m_newPosition);
            GameManager.Instance.Chunk.UpdateObjectPositionForChunks(target, m_oldPosition);
        }
    }

    public override void Undo()
    {
        m_position = m_oldPosition;
        m_rotation = m_oldRotation;

        ILayhgoObject target = GetTarget();
        if (target != null)
        {
            target.SetRotation(m_oldRotation.eulerAngles);
            target.SetPosition(m_oldPosition);
            GameManager.Instance.Chunk.UpdateObjectPositionForChunks(target, m_newPosition);
        }
    }
}

public class CommandManager : LayhgoManager
{
    private FiniteStack<ICommand> m_undo = new FiniteStack<ICommand>();
    private FiniteStack<ICommand> m_redo = new FiniteStack<ICommand>();

    /// <summary>
    /// Undo the first command in the undo list and add it to the redo list.
    /// </summary>
    public void Undo()
    {
        if (m_undo.Count > 0)
        {
            ICommand command = m_undo.Pop();
            command.Undo();

            m_redo.Push(command);
        }

        UpdateUI();
    }

    /// <summary>
    /// Redo the first command in the redo list and add it to the undo list.
    /// </summary>
    public void Redo()
    {
        if (m_redo.Count > 0)
        {
            ICommand command = m_redo.Pop();
            command.Redo();

            m_undo.Push(command);
        }
        UpdateUI();
    }

    /// <summary>
    /// Insert a command or commandgroup to the undo list.
    /// </summary>
    /// <param name="a_command"></param>
    public void Insert(ICommand a_command)
    {
        m_undo.Push(a_command);
        m_redo.Clear();
        UpdateUI();
    }
}