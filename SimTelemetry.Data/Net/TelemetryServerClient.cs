using System;
using System.Net.Sockets;
using SimTelemetry.Objects;

namespace SimTelemetry.Data.Net
{
    public class TelemetryServerClient
    {
        public bool Connected { get; set; }
        public bool Configured { get; set; }
        private TcpClient _mTcpConnection;
        private NetworkStream _mStream
        {
            get
            {
                if (_mTcpConnection == null || !_mTcpConnection.Connected)
                {
                    Connected = false;
                    return null;
                }
                return _mTcpConnection.GetStream();
            }
        }
        private byte[] _rxBuffer;
        private byte[] _txBuffer;
        private byte[] _txBufferBusy;
        private int _txPtr;

        public TelemetryServerClient(TcpClient tcp_connection)
        {
            Connected = true;
            Configured = false; // indicates whether received header info about game data

            _rxBuffer = new byte[16 * 1024];
            _txBuffer = new byte[128 * 1024];
            _txPtr = 0;

            _mTcpConnection = tcp_connection;
            try
            {
                if (_mStream != null)
                    _mStream.BeginRead(_rxBuffer, 0, 8, RxHeader, this);
            }
            catch (Exception ex)
            {

            }
        }

        private void RxHeader(IAsyncResult iar)
        {
            int read = 0;
            try
            {
                if (iar != null)
                {
                    if (_mStream != null)
                        read = _mStream.EndRead(iar);
                    //
                }
            }
            catch (Exception ex)
            {

            }
            try
            {
                if (_mStream != null)
                    _mStream.BeginRead(_rxBuffer, 0, 8, RxHeader, this);
            }
            catch (Exception ex)
            {

            }
        }

        private void TxDone(IAsyncResult iar)
        {
            try
            {
                if (iar != null)
                {
                    if (_mStream != null)
                        _mStream.EndWrite(iar);
                    // GREAT
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void PushGameData(byte[] data)
        {
            ByteMethods.memcpy(_txBuffer, data, data.Length, _txPtr, 0);
            Array.Copy(data, 0, _txBuffer, _txPtr, data.Length);
            _txPtr += data.Length;
        }

        public void FlushGameData()
        {
            _txBufferBusy = new byte[_txPtr];
            Array.Copy(_txBuffer, _txBufferBusy, _txBufferBusy.Length);

            _txPtr = 0;
            try
            {
                if (_mStream != null)
                    _mStream.BeginWrite(_txBufferBusy, 0, _txBufferBusy.Length, TxDone, this);
            }
            catch (Exception ex)
            {

            }


        }

        public void EndConnection()
        {
            _mTcpConnection.Close();
        }
    }
}
