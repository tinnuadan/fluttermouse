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
      Fast, Medium, Slow
    }
    #endregion

    #region Members
    private HotKey hotKey { get; set; }
    private InputSimulator inputSimulator { get; set; }
    private Timer timer { get; set; }
    private DispatcherTimer uiTimer { get; set; }
    private List<LabelProps> lblActiveProps { get; set; }
    //private List<AfkAction> actions { get; set; }

    private int run;
    //private AfkAction _currentAction;
    #endregion

    #region Public Methods

    public MainWindow()
    {
      InitializeComponent();
    }

    #endregion

    #region Private Methods
    private void cbMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      //var action = getSelectedAction();
      //cbSpeed.IsEnabled = (action.Type == AfkAction.ActionType.Repeated);
    }
    #endregion
  }
}
