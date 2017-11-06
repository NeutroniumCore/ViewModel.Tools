﻿using System.ComponentModel;
using FluentAssertions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Vm.Tools.Tests
{
    public class ViewModelTests
    {
        private readonly ViewModelTest _ViewModel;

        public ViewModelTests()
        {
            _ViewModel = new ViewModelTest();
        }

        [Theory, AutoData]
        public void Set_changes_property_value(string value)
        {
            _ViewModel.Property = value;

            _ViewModel.Property.Should().Be(value);
        }

        [Theory]
        [InlineData("a", "b")]
        [InlineData("a", "A")]
        public void Set_sends_propertyChanged(string from, string to)
        {
            _ViewModel.Property = from;

            _ViewModel.MonitorEvents();
            _ViewModel.Property = to;
            _ViewModel.ShouldRaisePropertyChangeFor(vm => vm.Property);
        }

        [Theory]
        [InlineData("a", "b")]
        [InlineData("a", "A")]
        public void Set_sends_propertyChanging(string from, string to)
        {
            _ViewModel.Property = from;

            _ViewModel.MonitorEvents();
            _ViewModel.Property = to;
            _ViewModel.ShouldRaise("PropertyChanging").WithArgs<PropertyChangingEventArgs>(evt => evt.PropertyName == nameof(ViewModelTest.Property));
        }

        [Theory, AutoData]
        public void Set_does_not_send_propertyChanged_when_value_is_the_same(string value)
        {
            _ViewModel.Property = value;

            _ViewModel.MonitorEvents();
            _ViewModel.Property = value;
            _ViewModel.ShouldNotRaisePropertyChangeFor(vm => vm.Property);
        }

        [Theory, AutoData]
        public void Set_does_not_send_propertyChanging_when_value_is_the_same(string value)
        {
            _ViewModel.Property = value;

            _ViewModel.MonitorEvents();
            _ViewModel.Property = value;
            _ViewModel.ShouldNotRaise("PropertyChanging");

        }

        [Theory]
        [InlineData("a", "b", true)]
        [InlineData("a", "A", true)]
        [InlineData("a", "a", false)]
        [InlineData(null, null, false)]
        public void Set_returns_true_if_value_changed(string from, string to, bool setValue)
        {
            _ViewModel.Property = from;
            _ViewModel.Property = to;

            _ViewModel.LastSetValue.Should().Be(setValue);
        }

        private class ViewModelTest : ViewModel
        {
            public bool? LastSetValue { get; private set; }
            private string _Property;


            public string Property
            {
                set { LastSetValue = Set(ref _Property, value); }
                get { return _Property; }
            }
        }
    }
}
