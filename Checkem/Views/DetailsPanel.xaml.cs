﻿using Checkem.CustomComponents;
using Checkem.Models;
using System;
using Sphere.Readable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;

namespace Checkem.Views
{
    public partial class DetailsPanel : UserControl, INotifyPropertyChanged
    {
        public DetailsPanel(Itembar itembar)
        {
            this.DataContext = this;

            //copy item bar
            this.itembar = itembar;

            //get item bar's todo properties
            this.todo = itembar.todo;

            InitializeComponent();

            CheckReminderState();
        }


        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Close;
        #endregion


        #region Variable
        public Itembar itembar;
        #endregion


        #region Property
        public Todo todo = new Todo();

        public string Title
        {
            get
            {
                return todo.Title;
            }
            set
            {
                //this will prevent user from saving task without title
                if (value != string.Empty)
                {
                    if (todo.Title != value)
                    {
                        todo.Title = value;

                        //update item bar's title text block's text
                        itembar.Update_Title();

                        OnPropertyChanged();
                    }
                }
            }
        }

        public string Description
        {
            get
            {
                return todo.Description;
            }
            set
            {
                if (todo.Description != value)
                {
                    todo.Description = value;

                    itembar.Update_Description();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsCompleted
        {
            get
            {
                return todo.IsCompleted;
            }
            set
            {
                if (todo.IsCompleted != value)
                {
                    todo.IsCompleted = value;

                    //update completion check box's check state in item bar
                    itembar.Update_IsCompleted();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsStarred
        {
            get
            {
                return todo.IsStarred;
            }
            set
            {
                if (todo.IsStarred != value)
                {
                    todo.IsStarred = value;

                    //update star toggle's check state in item bar
                    itembar.Update_IsStarred();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReminderOn
        {
            get
            {
                return todo.IsReminderOn;
            }
            set
            {
                if (todo.IsReminderOn != value)
                {
                    todo.IsReminderOn = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool IsAdvanceReminderOn
        {
            get
            {
                return todo.IsAdvanceReminderOn;
            }
            set
            {
                if (todo.IsAdvanceReminderOn != value)
                {
                    todo.IsAdvanceReminderOn = value;

                    OnPropertyChanged();
                }
            }
        }

        public DateTime BeginDateTime
        {
            get
            {
                return todo.BeginDateTime.Value;
            }
            set
            {
                if (todo.BeginDateTime.Value != value)
                {
                    todo.BeginDateTime = value;

                    itembar.Update_Reminder();
                    OnPropertyChanged();
                }
            }
        }

        public DateTime EndDateTime
        {
            get
            {
                return todo.EndDateTime.Value;
            }
            set
            {
                if (todo.EndDateTime.Value != value)
                {
                    todo.EndDateTime = value;

                    itembar.Update_Reminder();
                    OnPropertyChanged();
                }
            }
        }

        public string CreationDateTime
        {
            get
            {
                return $"Created on {DateTimeManipulator.SimplifiedDate(todo.CreationDateTime)}";
            }
        }
        #endregion


        private void CheckReminderState()
        {
            /* check if reminder is on, if it's on, check if advance reminder is on to determine reminder stete
             * 
             * IsReminder is false and IsAdvanceReminderOn is false => no reminder
             * IsReminder is true and IsAdvanceReminderOn is false  => basic reminder
             * IsReminder is true and IsAdvanceReminderOn is true   => advance reminder
             */

            if (IsReminderOn)
            {
                if (!IsAdvanceReminderOn)
                {
                    //show basic reminder's date time in date time picker
                    EndDatePicker.Text = $"{this.todo.EndDateTime.Value.Month}/{this.todo.EndDateTime.Value.Day}/{this.todo.EndDateTime.Value.Year}";
                    EndTimePicker.Text = $"{string.Format("{0:h:mm tt}", this.todo.EndDateTime)}";

                    SetReminder(ReminderState.Basic);
                }
                else
                {
                    //show advance reminder's date time in date time picker
                    BeginDatePicker.Text = $"{this.todo.BeginDateTime.Value.Month}/{this.todo.BeginDateTime.Value.Day}/{this.todo.BeginDateTime.Value.Year}";
                    BeginTimePicker.Text = $"{string.Format("{0:h:mm tt}", this.todo.BeginDateTime)}";

                    EndDatePicker.Text = $"{this.todo.EndDateTime.Value.Month}/{this.todo.EndDateTime.Value.Day}/{this.todo.EndDateTime.Value.Year}";
                    EndTimePicker.Text = $"{string.Format("{0:h:mm tt}", this.todo.EndDateTime)}";

                    SetReminder(ReminderState.Advance);
                }
            }
        }


        private void SetReminder(ReminderState reminderState)
        {
            //show the corresponding date time picker 
            switch (reminderState)
            {
                case ReminderState.None:
                    {
                        IsReminderOn = false;
                        IsAdvanceReminderOn = false;

                        BeginDateTimeField.Visibility = Visibility.Collapsed;
                        EndDateTimeField.Visibility = Visibility.Collapsed;

                        ReminderSelecter.SelectedIndex = 2;

                        break;
                    }
                case ReminderState.Basic:
                    {
                        IsReminderOn = true;
                        IsAdvanceReminderOn = false;

                        BeginDateTimeField.Visibility = Visibility.Collapsed;
                        EndDateTimeField.Visibility = Visibility.Visible;

                        ReminderSelecter.SelectedIndex = 0;

                        break;
                    }
                case ReminderState.Advance:
                    {
                        IsReminderOn = true;
                        IsAdvanceReminderOn = true;

                        BeginDateTimeField.Visibility = Visibility.Visible;
                        EndDateTimeField.Visibility = Visibility.Visible;

                        ReminderSelecter.SelectedIndex = 1;

                        break;
                    }
                default:
                    break;
            }
        }


        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        private void StoryBoard_Completed(object sender, EventArgs e)
        {
            Close?.Invoke(this, EventArgs.Empty);
        }

        private void ListBoxItem_NoReminder_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            SetReminder(ReminderState.None);
        }

        private void ListBoxItem_BasicReminder_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            SetReminder(ReminderState.Basic);
        }

        private void ListBoxItem_AdvanceReminder_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            SetReminder(ReminderState.Advance);
        }

        private void BeginDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            BeginDateTime = Convert.ToDateTime(BeginDatePicker.Text + " " + BeginDatePicker.Text);
        }

        private void BeginTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            BeginDateTime = Convert.ToDateTime(BeginDatePicker.Text + " " + BeginDatePicker.Text);
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            EndDateTime = Convert.ToDateTime(EndDatePicker.Text + " " + EndTimePicker.Text);
        }

        private void EndTimePicker_SelectedTimeChanged(object sender, RoutedPropertyChangedEventArgs<DateTime?> e)
        {
            EndDateTime = Convert.ToDateTime(EndDatePicker.Text + " " + EndTimePicker.Text);
        }
    }
}
