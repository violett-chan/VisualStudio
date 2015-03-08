﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using GitHub.Exports;
using GitHub.Models;
using GitHub.UI;
using GitHub.UI.Helpers;
using GitHub.ViewModels;
using NullGuard;
using ReactiveUI;

namespace GitHub.VisualStudio.UI.Views.Controls
{
    /// <summary>
    /// Interaction logic for CloneRepoControl.xaml
    /// </summary>
    [ExportView(ViewType=UIViewType.Clone)]
    public partial class CloneRepositoryControl : IViewFor<ICloneRepositoryViewModel>, IView
    {
        public CloneRepositoryControl()
        {
            SharedDictionaryManager.Load("GitHub.UI");
            SharedDictionaryManager.Load("GitHub.UI.Reactive");
            Resources.MergedDictionaries.Add(SharedDictionaryManager.SharedDictionary);

            InitializeComponent();

            DataContextChanged += (s, e) => ViewModel = e.NewValue as ICloneRepositoryViewModel;

            this.WhenActivated(d =>
            {
                d(this.OneWayBind(ViewModel, vm => vm.Repositories, v => v.repositoryList.ItemsSource, CreateRepositoryListCollectionView));
                d(this.BindCommand(ViewModel, vm => vm.CloneCommand, v => v.cloneButton));
            });
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
           "ViewModel", typeof(ICloneRepositoryViewModel), typeof(CloneRepositoryControl), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ICloneRepositoryViewModel)value; }
        }

        object IView.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ICloneRepositoryViewModel)value; }
        }

        public ICloneRepositoryViewModel ViewModel
        {
            [return: AllowNull]
            get { return (ICloneRepositoryViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        static ListCollectionView CreateRepositoryListCollectionView(ICollection<IRepositoryModel> repositories)
        {
            var view = new ListCollectionView((IList)repositories);
            view.GroupDescriptions.Add(new RepositoryGroupDescription());
            return view;
        }

        class RepositoryGroupDescription : GroupDescription
        {
            public override object GroupNameFromItem(object item, int level, System.Globalization.CultureInfo culture)
            {
                return ((IRepositoryModel)item).Owner;
            }

            public override bool NamesMatch(object groupName, object itemName)
            {
                return string.Equals((string)groupName, (string)itemName);
            }
        }
    }
}
