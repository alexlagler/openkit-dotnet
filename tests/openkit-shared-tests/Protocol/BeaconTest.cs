﻿//
// Copyright 2018 Dynatrace LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Dynatrace.OpenKit.Providers;
using NUnit.Framework;
using NSubstitute;
using Dynatrace.OpenKit.Core;
using Dynatrace.OpenKit.API;
using Dynatrace.OpenKit.Core.Caching;

namespace Dynatrace.OpenKit.Protocol
{
    public class BeaconTest
    {
        private IThreadIDProvider threadIdProvider;
        private ITimingProvider timingProvider;

        [SetUp]
        public void Setup()
        {
            threadIdProvider = Substitute.For<IThreadIDProvider>();
            timingProvider = Substitute.For<ITimingProvider>();
        }

        [Test]
        public void CanAddUserIdentifyEvent()
        {
            // given
            var beacon = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var userTag = "myTestUser";

            // when
            beacon.IdentifyUser(userTag);
            var target = beacon.EventDataList;

            // then
            Assert.That(target, Is.EquivalentTo(new[] { $"et=60&na={userTag}&it=0&pa=0&s0=1&t0=0" }));
        }

        [Test]
        public void CanAddSentBytesToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);
            var bytesSent = 123;

            //when
            webRequest.Start().SetBytesSent(bytesSent).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0&bs={bytesSent}" }));
        }

        [Test]
        public void CanAddSentBytesValueZeroToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);
            var bytesSent = 0;

            //when
            webRequest.Start().SetBytesSent(bytesSent).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0&bs={bytesSent}" }));
        }

        [Test]
        public void CannotAddSentBytesWithInvalidValueSmallerZeroToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);

            //when
            webRequest.Start().SetBytesSent(-1).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0" }));
        }

        [Test]
        public void CanAddReceivedBytesToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);
            var bytesReceived = 12321;

            //when
            webRequest.Start().SetBytesReceived(bytesReceived).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0&br={bytesReceived}" }));
        }

        [Test]
        public void CanAddReceivedBytesValueZeroToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);
            var bytesReceived = 0;

            //when
            webRequest.Start().SetBytesReceived(bytesReceived).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0&br={bytesReceived}" }));
        }

        [Test]
        public void CannotAddReceivedBytesWithInvalidValueSmallerZeroToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);

            //when
            webRequest.Start().SetBytesReceived(-1).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0" }));
        }

        [Test]
        public void CanAddBothSentBytesAndReceivedBytesToWebRequestTracer()
        {
            //given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestRootAction", new SynchronizedQueue<IAction>());
            var testURL = "127.0.0.1";
            var webRequest = new WebRequestTracerStringURL(target, action, testURL);
            var bytesReceived = 12321;
            var bytesSent = 123;

            //when
            webRequest.Start().SetBytesSent(bytesSent).SetBytesReceived(bytesReceived).Stop(); //stop will add the web request to the beacon

            //then
            Assert.That(target.EventDataList, Is.EquivalentTo(new[] { $"et=30&na={testURL}&it=0&pa=1&s0=2&t0=0&s1=3&t1=0&bs=123&br=12321" }));
        }

        [Test]
        public void CanAddRootActionIfCaptureIsOn()
        {
            // given
            var configuration = new TestConfiguration();
            configuration.EnableCapture();

            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), 
                configuration, "127.0.0.1", threadIdProvider, timingProvider);
            const string RootActionName = "TestRootAction";


            // when adding the root action
            var action = new RootAction(target, RootActionName, new SynchronizedQueue<IAction>()); // the action is added to the beacon in the constructor
            action.LeaveAction();

            // then
            Assert.That(target.ActionDataList, Is.EquivalentTo(new[] { $"et=1&na={RootActionName}&it=0&ca=1&pa=0&s0=1&t0=0&s1=2&t1=0" }));
        }

        [Test]
        public void CannotAddRootActionIfCaptureIsOff()
        {
            // given
            var configuration = new TestConfiguration();
            configuration.DisableCapture();

            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), 
                configuration, "127.0.0.1", threadIdProvider, timingProvider);

            // when adding the root action
            const string RootActionName = "TestRootAction";
            target.AddAction(new RootAction(target, RootActionName, new SynchronizedQueue<IAction>()));

            // then
            Assert.That(target.ActionDataList, Is.Empty);
        }

        [Test]
        public void ClearDataClearsActionAndEventData()
        {
            // given
            var target = new Beacon(Substitute.For<ILogger>(), new BeaconCache(), new TestConfiguration(), "127.0.0.1", threadIdProvider, timingProvider);
            var action = new Action(target, "TestAction", new SynchronizedQueue<IAction>());
            action.ReportEvent("TestEvent").ReportValue("TheAnswerToLifeTheUniverseAndEverything", 42);
            target.AddAction(action);

            // then check data both lists are not empty
            Assert.That(target.ActionDataList, Is.Not.Empty);
            Assert.That(target.EventDataList, Is.Not.Empty);

            // and when clearing the data
            target.ClearData();

            // then check data both lists are emtpy
            Assert.That(target.ActionDataList, Is.Empty);
            Assert.That(target.EventDataList, Is.Empty);
        }
    }
}
