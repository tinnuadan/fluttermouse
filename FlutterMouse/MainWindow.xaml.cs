using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput.Native;
using WindowsInput;
using System.Windows.Threading;
using System.Timers;

namespace FlutterMouse
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region Private Structs
    private class LabelProps
    {
      public Brush FG { get; set; }
      public Brush BG { get; set; }

      public LabelProps(Brush fg, Brush bg)
      {
        FG = fg;
        BG = bg;
      }
    }

    private enum Speed
    {
      Slow, Medium, Fast
    }
    #endregion

    #region Members
    private Dictionary<Speed, int> Speed2Time { get; set; }
    private HotKey hotKey { get; set; }
    private InputSimulator inputSimulator { get; set; }
    private Timer timer { get; set; }
    private DispatcherTimer uiTimer { get; set; }
    private List<LabelProps> lblActiveProps { get; set; }
    private List<MouseAction> actions { get; set; }

    private int run;
    private MouseAction _currentAction;
    #endregion

    #region Public Methods

    public MainWindow()
    {
      InitializeComponent();
      hotKey = new HotKey(Key.F9, KeyModifier.Alt, OnHotKeyHandler);
      inputSimulator = new InputSimulator();
      timer = new Timer();
      timer.AutoReset = true;
      timer.Elapsed += timerElapsed;
      uiTimer = new DispatcherTimer();
      uiTimer.Interval = TimeSpan.FromMilliseconds(500);
      uiTimer.Tick += uiTimerElapsed;
      run = -1;

      Speed2Time = new Dictionary<Speed, int>();
      Speed2Time.Add(Speed.Slow, 1000);
      Speed2Time.Add(Speed.Medium, 500);
      Speed2Time.Add(Speed.Fast, 250);



      lblActiveProps = new List<LabelProps>();
      lblActiveProps.Add(new LabelProps(lblActive.Foreground, lblActive.Background));
      lblActiveProps.Add(new LabelProps(new SolidColorBrush(Color.FromRgb(255, 255, 255)), new SolidColorBrush(Color.FromRgb(255, 0, 0))));
      lblActiveProps.Add(new LabelProps(new SolidColorBrush(Color.FromRgb(255, 0, 0)), new SolidColorBrush(Color.FromRgb(255, 255, 255))));

      actions = new List<MouseAction>();
      actions.Add(new MouseAction("Click Left Mouse Button", () => { inputSimulator.Mouse.LeftButtonClick(); return true; }));
      actions.Add(new MouseAction("Press Left Mouse Button", () => { inputSimulator.Mouse.LeftButtonDown(); return true; }, () => { inputSimulator.Mouse.LeftButtonUp(); return true; }));
      actions.Add(new MouseAction("Click Right Mouse Button", () => { inputSimulator.Mouse.RightButtonClick(); return true; }));
      actions.Add(new MouseAction("Press Right Mouse Button", () => { inputSimulator.Mouse.RightButtonDown(); return true; }, () => { inputSimulator.Mouse.RightButtonUp(); return true; }));
      _currentAction = null;

      foreach (var action in actions)
      {
        cbMode.Items.Add(action.Description);
      }


      cbSpeed.SelectedIndex = 1;
      cbMode.SelectedIndex = 0;
    }


    #endregion

    #region Private Methods
    private MouseAction getSelectedAction()
    {
      int index = cbMode.SelectedIndex;
      return actions[index];
    }

    private Speed getSpeed()
    {
      int index = cbSpeed.SelectedIndex;
      return (Speed)index;
    }

    private int speedToTime(Speed s)
    {
      int speed = 0;
      Speed2Time.TryGetValue(s, out speed);
      return speed;
    }

    private void uiTimerElapsed(object sender, EventArgs e)
    {
      formatActiveLabel(run % 2 + 1);
      run++;
    }

    private void timerElapsed(object sender, ElapsedEventArgs e)
    {
      if (_currentAction != null)
      {
        _currentAction.doaction();
      }
    }

    private void OnHotKeyHandler(HotKey obj)
    {
      if (run >= 0)
      {
        stopAction();
      }
      else
      {
        startAction();
      }
    }

    private void startAction()
    {
      cbMode.IsEnabled = false;
      var freq = speedToTime(getSpeed());
      _currentAction = getSelectedAction();

      if (_currentAction.Type == MouseAction.ActionType.Constant)
      {
        _currentAction.enter();
      }
      else
      {
        timer.Interval = speedToTime(getSpeed());
        timer.Start();
      }
      uiTimer.Start();
      run = 0;
      lblActive.Content = "ACTIVE";
      formatActiveLabel(2);
    }


    private void stopAction()
    {
      if (_currentAction == null)
      {
        return;
      }
      timer.Stop();
      if (_currentAction.Type == MouseAction.ActionType.Constant)
      {
        _currentAction.leave();
      }
      uiTimer.Stop();
      _currentAction = null;
      run = -1;
      lblActive.Content = "Inactive";
      formatActiveLabel(0);
      cbMode.IsEnabled = true;
    }

    private void formatActiveLabel(int i)
    {
      lblActive.Foreground = lblActiveProps[i].FG;
      lblActive.Background = lblActiveProps[i].BG;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      stopAction();
    }

    private void cbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var action = getSelectedAction();
      cbSpeed.IsEnabled = (action.Type == MouseAction.ActionType.Repeated);
    }
    #endregion
  }
}
