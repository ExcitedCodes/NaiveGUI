﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NaiveGUI.Model
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected Dictionary<string, HashSet<string>> SourceBinding = new Dictionary<string, HashSet<string>>();

        public ModelBase()
        {
            foreach (var prop in GetType().GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attr = prop.GetCustomAttribute<SourceBindingAttribute>();
                if (attr == null)
                {
                    continue;
                }
                foreach (var source in attr.Sources)
                {
                    if (!SourceBinding.ContainsKey(source))
                    {
                        SourceBinding[source] = new HashSet<string>();
                    }
                    SourceBinding[source].Add(prop.Name);
                }
            }
        }

        protected void Set<T>(out T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            RaisePropertyChanged(propertyName);
        }

        protected bool RaisePropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (SourceBinding.ContainsKey(name))
            {
                foreach (var affected in SourceBinding[name])
                {
                    RaisePropertyChanged(affected);
                }
            }
            return PropertyChanged == null;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SourceBindingAttribute : Attribute
    {
        public IEnumerable<string> Sources { get; private set; }

        public SourceBindingAttribute(params string[] sources)
        {
            Sources = sources;
        }
    }
}
