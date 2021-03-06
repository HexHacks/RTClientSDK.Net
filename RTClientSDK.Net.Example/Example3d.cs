// Realtime SDK Example for Qualisys Track Manager. Copyright 2017 Qualisys AB
//
using QTMRealTimeSDK;
using QTMRealTimeSDK.Data;
using System;
using System.Threading;

namespace RTClientSDK.Net.Example
{
    class Example3D : Example
    {
        public Example3D(RTProtocol rtProtocol, string ipAddress) : base(rtProtocol, ipAddress)
        {
        }

        public override void HandleStreaming()
        {
            // Check if connection to QTM is possible
            if (!mRtProtocol.IsConnected())
            {
                if (!mRtProtocol.Connect(mIpAddress))
                {
                    Console.WriteLine("QTM: Trying to connect");
                    Thread.Sleep(1000);
                    return;
                }
                Console.WriteLine("QTM: Connected");
            }

            // Check for available 3DOF with residual data in the stream
            if (mRtProtocol.Settings3D == null)
            {
                if (!mRtProtocol.Get3dSettings())
                {
                    Console.WriteLine("QTM: Trying to get marker settings");
                    Thread.Sleep(500);
                    return;
                }
                Console.WriteLine("QTM: Marker settings available");

                foreach (var identifiedMarkers in mRtProtocol.Settings3D.labels3D)
                {
                    Console.WriteLine("{0}", identifiedMarkers.Name);
                }

                mRtProtocol.StreamAllFrames(QTMRealTimeSDK.Data.ComponentType.Component3dResidual);
                Console.WriteLine("QTM: Starting to stream marker data");
                Thread.Sleep(500);
            }

            // Get RTPacket from stream
            PacketType packetType;
            mRtProtocol.ReceiveRTPacket(out packetType, false);

            // Handle data packet
            if (packetType == PacketType.PacketData)
            {
                var threeDofData = mRtProtocol.GetRTPacket().Get3DMarkerResidualData();
                if (threeDofData != null)
                {
                    Console.WriteLine(mRtProtocol.GetRTPacket().Get3DMarkerResidualData().Count);
                    for (int body = 0; body < mRtProtocol.GetRTPacket().Get3DMarkerResidualData().Count; body++)
                    {
                        var threeDofBody = threeDofData[body];
                        Console.WriteLine("Frame:{0:D5} X:{1,7:F1} Y:{2,7:F1} Z:{3,7:F1} Residual:{4,7:F1}",
                            mRtProtocol.GetRTPacket().Frame,
                            threeDofBody.Position.X, threeDofBody.Position.Y, threeDofBody.Position.Z,
                            threeDofBody.Residual);
                    }
                }
            }

            // Handle event packet
            if (packetType == PacketType.PacketEvent)
            {
                // If an event comes from QTM then print it out
                var qtmEvent = mRtProtocol.GetRTPacket().GetEvent();
                Console.WriteLine("{0}", qtmEvent);
            }
        }
    }
}