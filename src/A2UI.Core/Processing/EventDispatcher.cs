using A2UI.Core.Messages;

namespace A2UI.Core.Processing;

/// <summary>
/// Handles dispatching of user actions and errors to the server.
/// </summary>
public class EventDispatcher
{
    /// <summary>
    /// Event raised when a user action occurs.
    /// </summary>
    public event EventHandler<UserActionEventArgs>? UserActionDispatched;

    /// <summary>
    /// Event raised when an error occurs.
    /// </summary>
    public event EventHandler<ErrorEventArgs>? ErrorDispatched;

    /// <summary>
    /// Event raised when data is updated from a component.
    /// </summary>
    public event EventHandler<DataUpdateEventArgs>? DataUpdateDispatched;

    /// <summary>
    /// Dispatches a user action.
    /// </summary>
    public void DispatchUserAction(UserActionMessage action)
    {
        UserActionDispatched?.Invoke(this, new UserActionEventArgs(action));
    }

    /// <summary>
    /// Dispatches an error.
    /// </summary>
    public void DispatchError(Dictionary<string, object> error)
    {
        ErrorDispatched?.Invoke(this, new ErrorEventArgs(error));
    }

    /// <summary>
    /// Dispatches a data update event.
    /// </summary>
    public void DispatchDataUpdate(DataUpdateMessage update)
    {
        DataUpdateDispatched?.Invoke(this, new DataUpdateEventArgs(update));
    }

    /// <summary>
    /// Creates a user action message.
    /// </summary>
    public static UserActionMessage CreateUserAction(
        string actionName,
        string surfaceId,
        string sourceComponentId,
        Dictionary<string, object> context)
    {
        return new UserActionMessage
        {
            Name = actionName,
            SurfaceId = surfaceId,
            SourceComponentId = sourceComponentId,
            Timestamp = DateTime.UtcNow,
            Context = context
        };
    }

    /// <summary>
    /// Creates a client-to-server message with a user action.
    /// </summary>
    public static ClientToServerMessage CreateUserActionMessage(UserActionMessage action)
    {
        return new ClientToServerMessage
        {
            UserAction = action
        };
    }

    /// <summary>
    /// Creates a client-to-server message with an error.
    /// </summary>
    public static ClientToServerMessage CreateErrorMessage(Dictionary<string, object> error)
    {
        return new ClientToServerMessage
        {
            Error = error
        };
    }

    /// <summary>
    /// Creates a data update message for component data changes.
    /// </summary>
    public static DataUpdateMessage CreateDataUpdate(
        string surfaceId,
        string componentId,
        string path,
        object value)
    {
        return new DataUpdateMessage
        {
            SurfaceId = surfaceId,
            ComponentId = componentId,
            Path = path,
            Value = value,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Event args for user actions.
/// </summary>
public class UserActionEventArgs : EventArgs
{
    public UserActionMessage Action { get; }

    public UserActionEventArgs(UserActionMessage action)
    {
        Action = action;
    }
}

/// <summary>
/// Event args for errors.
/// </summary>
public class ErrorEventArgs : EventArgs
{
    public Dictionary<string, object> Error { get; }

    public ErrorEventArgs(Dictionary<string, object> error)
    {
        Error = error;
    }
}

/// <summary>
/// Event args for data updates.
/// </summary>
public class DataUpdateEventArgs : EventArgs
{
    public DataUpdateMessage Update { get; }

    public DataUpdateEventArgs(DataUpdateMessage update)
    {
        Update = update;
    }
}

/// <summary>
/// Message for data updates from components.
/// </summary>
public class DataUpdateMessage
{
    public string SurfaceId { get; set; } = "";
    public string ComponentId { get; set; } = "";
    public string Path { get; set; } = "";
    public object? Value { get; set; }
    public DateTime Timestamp { get; set; }
}

