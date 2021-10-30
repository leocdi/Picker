using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Picker.Control;
using Picker.Droid.Renderer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PickerFix), typeof(PickerFixRenderer))]
namespace Picker.Droid.Renderer
{
    public class PickerFixRenderer : Xamarin.Forms.Platform.Android.AppCompat.PickerRenderer
    {
        AlertDialog _dialog;
        bool _isSet;

        public PickerFixRenderer(Context context) : base(context)
        {
        }

        // This method is necessary to set the OnClickListener, copied removing base.OnElementChanged(e); line
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Picker> e)
        {
            base.OnElementChanged(e);
            if (Control != null && !_isSet)
            {

                Control.SetOnClickListener(PickerListener.Instance);
                Control.Tag = this;
                _isSet = true;
            }
        }

        // This method is necessary to change negative button text.
        void MOnClick()
        {
            Xamarin.Forms.Picker model = Element;
            if (_dialog == null)
            {
                using (var builder = new AlertDialog.Builder(Context))
                {
                    builder.SetTitle(model.Title ?? "");
                    string[] items = model.Items.ToArray();

                    items[model.SelectedIndex] = $"➡️  {items[model.SelectedIndex]}  ⬅️";

                    builder.SetItems(items, (s, e) => ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Picker.SelectedIndexProperty, e.Which));
               
                    //builder.SetSingleChoiceItems(items, model.SelectedIndex, (s, e) => ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Picker.SelectedIndexProperty, e.Which));
                    builder.SetNegativeButton(global::Android.Resource.String.Cancel, (o, args) => { });

                    ((IElementController)Element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);

                    _dialog = builder.Create();

                    _dialog.SetOnShowListener(AlertDialogListener.Instance);
                }
                _dialog.SetCanceledOnTouchOutside(true);
                _dialog.DismissEvent += (sender, args) =>
                {
                    (Element as IElementController)?.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
                    _dialog.Dispose();
                    _dialog = null;
                };

                _dialog.ListView.Tag = model.SelectedIndex;

                _dialog.Show();
            }
        }


        void RowsCollectionChanged(object sender, EventArgs e)
        {
            UpdatePicker();
        }

        void UpdatePicker()
        {
            Control.Hint = Element.Title;

            if (Element.SelectedIndex == -1 || Element.Items == null)
                Control.Text = null;
            else
                Control.Text = Element.Items[Element.SelectedIndex];
        }



        // The listener is changed to work with CustomPickerRenderer
        class PickerListener : Java.Lang.Object, IOnClickListener
        {
            #region Statics

            public static readonly PickerListener Instance = new PickerListener();

            #endregion

            public void OnClick(global::Android.Views.View v)
            {
                var renderer = v.Tag as PickerFixRenderer; // Work with my renderer
                renderer?.MOnClick();
            }
        }

        class AlertDialogListener : Java.Lang.Object, IDialogInterfaceOnShowListener
        {
            #region Statics

            public static readonly AlertDialogListener Instance = new AlertDialogListener();

            #endregion

            public void OnShow(IDialogInterface dialog)
            {
                var l_dlgAlert = dialog as AlertDialog;

                if (l_dlgAlert.ListView?.Tag == null) return;

                var l_nSelectedIndex = (int)l_dlgAlert.ListView.Tag;

                if (l_nSelectedIndex < 0) return;

                l_dlgAlert.ListView.SetSelection(l_nSelectedIndex);


                int l_nHeight = l_dlgAlert.ListView.MeasuredHeight;

                l_dlgAlert.ListView.SmoothScrollToPositionFromTop(l_nSelectedIndex, l_nHeight / 2);
            }



        }
    }
}

namespace Xamarin.Forms.Platform.Android
{
    /// <summary>
    /// Handles color state management for the TextColor property 
    /// for Entry, Button, Picker, TimePicker, and DatePicker
    /// </summary>
    internal class TextColorSwitcher
    {
        static readonly int[][] s_colorStates = { new[] { global::Android.Resource.Attribute.StateEnabled }, new[] { -global::Android.Resource.Attribute.StateEnabled } };

        readonly ColorStateList _defaultTextColors;
        readonly bool _useLegacyColorManagement;
        Color _currentTextColor;

        public TextColorSwitcher(ColorStateList textColors, bool useLegacyColorManagement = true)
        {
            _defaultTextColors = textColors;
            _useLegacyColorManagement = useLegacyColorManagement;
        }

        public void UpdateTextColor(TextView control, Color color, Action<ColorStateList> setColor = null)
        {
            if (color == _currentTextColor)
                return;

            if (setColor == null)
            {
                setColor = control.SetTextColor;
            }

            _currentTextColor = color;

            if (color.IsDefault)
            {
                setColor(_defaultTextColors);
            }
            else
            {
                if (_useLegacyColorManagement)
                {
                    // Set the new enabled state color, preserving the default disabled state color
                    int defaultDisabledColor = _defaultTextColors.GetColorForState(s_colorStates[1], color.ToAndroid());
                    setColor(new ColorStateList(s_colorStates, new[] { color.ToAndroid().ToArgb(), defaultDisabledColor }));
                }
                else
                {
                    var acolor = color.ToAndroid().ToArgb();
                    setColor(new ColorStateList(s_colorStates, new[] { acolor, acolor }));
                }
            }
        }
    }
}