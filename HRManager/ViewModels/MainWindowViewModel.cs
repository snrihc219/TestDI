﻿using System;
using System.Collections.Generic;
using System.Text;
using MesBase.Mvvm;
using HumanResource;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HRManager.ViewModels
{
    /// <summary>
    /// MainWindow的ViewModel
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        #region Properties
        private IHumanCenter HumanCenter { get; set; }
        private IPersonFactory PersonFactory { get; set; }
        private IOrganizationFactory OrganizationFactory { get; set; }
        #endregion

        public MainWindowViewModel(IServiceProvider sp)
        {
            this.HumanCenter = sp.GetService(typeof(IHumanCenter)) as IHumanCenter;
            this.PersonFactory = sp.GetService(typeof(IPersonFactory)) as IPersonFactory;
            this.OrganizationFactory = sp.GetService(typeof(IOrganizationFactory)) as IOrganizationFactory;
            this.Organizations = new ObservableCollection<Organization>();
            HumanCenter.GetOrganizations().ForEach(org => Organizations.Add(org as Organization));
            var names = new List<string>();
            foreach (var o in Organizations) 
                names.Add(o.Name);
            //this.OrganizationNames = new ObservableCollection<string>(names);
            OrganizationTypes = new ObservableCollection<string>(new List<string>
            {
                "School", "Company"
            });
            PersonTypes = new ObservableCollection<string>(new List<string>
            {
                "Student", "Worker"
            });
            PersonGenders = new ObservableCollection<string>(new List<string>
            {
                "Male", "Female"
            });
        }

        #region ObservableObjects
        /// <summary>
        /// 全部机构，绑定主表格
        /// </summary>
        public ObservableCollection<Organization> Organizations { get; set; }
        /// <summary>
        /// 可能的机构类型，绑定下拉菜单
        /// </summary>
        public ObservableCollection<string> OrganizationTypes { get; set; }
        /// <summary>
        /// 可能的Person类别，绑定下拉菜单
        /// </summary>
        public ObservableCollection<string> PersonTypes { get; set; }
        /// <summary>
        /// 可能的Person性别，绑定下拉菜单
        /// </summary>
        public ObservableCollection<string> PersonGenders { get; set; }

        private string _organizationName = "ZJU";
        /// <summary>
        /// 机构名，绑定表格中选中机构名称
        /// </summary>
        public string OrganizationName
        {
            get => _organizationName;
            set => SetProperty(ref _organizationName, value);
        }
        private string _organizationType = "School";
        /// <summary>
        /// 可能的机构类型，绑定下拉菜单
        /// </summary>
        public string OrganizationType
        {
            get => _organizationType;
            set => SetProperty(ref _organizationType, value);
        }
        private string _personType = "Student";
        /// <summary>
        /// 人的类别，绑定下拉菜单中选中的类别
        /// </summary>
        public string PersonType
        {
            get => _personType;
            set => SetProperty(ref _personType, value);
        }
        private string _personName = "Wang";
        /// <summary>
        /// 人名，绑定对应文本框
        /// </summary>
        public string PersonName
        {
            get => _personName;
            set => SetProperty(ref _personName, value);
        }
        private string _personGender = "Male";
        /// <summary>
        /// 性别，绑定下拉菜单中选中的性别
        /// </summary>
        public string PersonGender
        {
            get => _personGender;
            set => SetProperty(ref _personGender, value);
        }
        private int _personAge = 18;
        /// <summary>
        /// 年龄，绑定对应文本框
        /// </summary>
        public int PersonAge
        {
            get => _personAge;
            set => SetProperty(ref _personAge, value);
        }
        private Organization _currentOrganization;
        /// <summary>
        /// 当前选中机构，绑定主表格中的选中项目
        /// </summary>
        public Organization CurrentOrganization
        {
            get => _currentOrganization;
            set => SetProperty(ref _currentOrganization, value);
        }
        private IPerson _currentPerson;
        /// <summary>
        /// 当前选中人，绑定子表格中的选中项目
        /// </summary>
        public IPerson CurrentPerson
        {
            get => _currentPerson;
            set => SetProperty(ref _currentPerson, value);
        }
        /// <summary>
        /// 增加Organization
        /// </summary>
        public ICommand AddOrganizationCommand => new RelayCommand(() =>
        {
            //定义person加入organization后的处理事项
            var onEntered = new Action<IOrganization, IPerson>((org, p) =>
            {
                p.Organization = org.Name;
                (org as Organization).PeopleToShow = new ObservableCollection<IPerson>(org.GetMembers());
            });
            //定义person离开organization后的处理事项
            var onLeft = new Action<IOrganization, IPerson>((org, p) =>
            {
                p.Organization = string.Empty;
                (org as Organization).PeopleToShow = new ObservableCollection<IPerson>(org.GetMembers());
            });
            try
            {
                var org = OrganizationFactory.CreateOrganization(OrganizationType, OrganizationName, onEntered, onLeft);
                HumanCenter.AddOrganization(org);
                var list = HumanCenter.GetOrganizations();
                Organizations.Clear();
                list.ForEach(o => Organizations.Add(o as Organization));
            }
            catch (Exception) { }
        });
        /// <summary>
        /// 移除Organization
        /// </summary>
        public ICommand RemoveOrganizationCommand => new RelayCommand(() =>
        {
            if (CurrentOrganization != null)
                HumanCenter.RemoveOrganization(CurrentOrganization.Id);
            var list = HumanCenter.GetOrganizations();
            Organizations.Clear();
            list.ForEach(o => Organizations.Add(o as Organization));
        });
        /// <summary>
        /// 在指定机构中增加一个成员
        /// </summary>
        public ICommand AddPersonCommand => new RelayCommand(() =>
        {
            //var currentOrganization = HumanCenter.GetOrganizationByName(OrganizationName);
            if(CurrentOrganization != null)
            {
                var person = PersonFactory.CreatePerson(PersonType, PersonName, PersonGender, PersonAge);
                CurrentOrganization.AddMember(person);
            }
        });
        /// <summary>
        /// 在指定机构中移除一个指定成员
        /// </summary>
        public ICommand RemovePersonCommand => new RelayCommand(() =>
        {
            if (CurrentOrganization != null)
            {
                CurrentOrganization.RemoveMember(CurrentPerson);
            }
        });
        #endregion
    }
}
