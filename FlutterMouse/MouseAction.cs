using System;

namespace FlutterMouse
{
  class MouseAction
  {
    public enum ActionType { Constant, Repeated }
    public string Description { get; set; }
    public ActionType Type { get; private set; }

    private Func<bool> EnterAction { get; set; }
    private Func<bool> LeaveAction { get; set; }
    private Func<bool> RepeateAction { get; set; }

    public MouseAction(string desc, Func<bool> enterAction, Func<bool> leaveAction)
    {
      Description = desc;
      Type = ActionType.Constant;
      EnterAction = enterAction;
      LeaveAction = leaveAction;
      RepeateAction = () => true;
    }
    public MouseAction(string desc, Func<bool> repeatedAction)
    {
      Description = desc;
      Type = ActionType.Repeated;
      EnterAction = () => true;
      LeaveAction = () => true;
      RepeateAction = repeatedAction;
    }

    public bool enter()
    {
      return EnterAction();
    }
    public bool leave()
    {
      return LeaveAction();
    }
    public bool doaction()
    {
      return RepeateAction();
    }
  }
}
