using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace HTTP
{
	public class WebSocket
	{
		public delegate void OnTextMessageHandler(string message);

		public delegate void OnBinaryMessageHandler(byte[] message);

		[Flags]
		public enum OpCode
		{
			OpCodeContinuation = 0,
			OpCodeText = 1,
			OpCodeBinary = 2,
			OpCodeClose = 8,
			OpCodePing = OpCodeText | OpCodeClose,
			OpCodePong = OpCodeBinary | OpCodeClose
		}

		private enum ParseFrameResult
		{
			FrameIncomplete = 0,
			FrameOK = 1,
			FrameError = 2
		}

		public enum CloseEventCode
		{
			CloseEventCodeNotSpecified = -1,
			CloseEventCodeNormalClosure = 1000,
			CloseEventCodeGoingAway = 1001,
			CloseEventCodeProtocolError = 1002,
			CloseEventCodeUnsupportedData = 1003,
			CloseEventCodeFrameTooLarge = 1004,
			CloseEventCodeNoStatusRcvd = 1005,
			CloseEventCodeAbnormalClosure = 1006,
			CloseEventCodeInvalidUTF8 = 1007,
			CloseEventCodeMinimumUserDefined = 3000,
			CloseEventCodeMaximumUserDefined = 4999
		}

		private class FrameData
		{
			public OpCode opCode;

			public bool final;

			public bool reserved1;

			public bool reserved2;

			public bool reserved3;

			public bool masked;

			public int payload;

			public int payloadLength;

			public int end;
		}

		private class SubArray : IEnumerable<byte>, IEnumerable
		{
			private List<byte> array;

			private int offset;

			private int length;

			public SubArray(List<byte> array, int offset, int length)
			{
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return null;
			}

			public IEnumerator<byte> GetEnumerator()
			{
				return null;
			}
		}

		private class SubArrayEnum : IEnumerator<byte>, IEnumerator, IDisposable
		{
			private List<byte> array;

			private int offset;

			private int length;

			private int position;

			object IEnumerator.Current => null;

			public byte Current => 0;

			public SubArrayEnum(List<byte> array, int offset, int length)
			{
			}

			public bool MoveNext()
			{
				return false;
			}

			public void Reset()
			{
			}

			public void Dispose()
			{
			}
		}

		public struct OutgoingMessage
		{
			public readonly OpCode opCode;

			public readonly byte[] data;

			public OutgoingMessage(OpCode opCode, byte[] data)
			{
				this.opCode = OpCode.OpCodeContinuation;
				this.data = null;
			}
		}

		[CompilerGenerated]
		private sealed class _003CDispatcher_003Ed__44 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public WebSocket _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			[DebuggerHidden]
			public _003CDispatcher_003Ed__44(int _003C_003E1__state)
			{
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_Wait_003Ed__47 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public WebSocket _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			[DebuggerHidden]
			public _003C_Wait_003Ed__47(int _003C_003E1__state)
			{
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
			}
		}

		public int niceness;

		private const byte FINALBIT = 128;

		private const byte RESERVEDBIT1 = 64;

		private const byte RESERVEDBIT2 = 32;

		private const byte RESERVEDBIT3 = 16;

		private const byte OP_CODE_MASK = 15;

		private const byte MASKBIT = 128;

		private const byte PAYLOAD_LENGTH_MASK = 127;

		private const int MASKING_KEY_WIDTH_IN_BYTES = 4;

		private const int MAX_PAYLOAD_WITHOUT_EXTENDED_LENGTH_FIELD = 125;

		private const int PAYLOAD_WITH_TWO_BYTE_EXTENDED_FIELD = 126;

		private const int PAYLOAD_WITH_EIGHT_BYTE_EXTENDED_FIELD = 127;

		[CompilerGenerated]
		private OnTextMessageHandler m_OnTextMessageRecv;

		[CompilerGenerated]
		private OnBinaryMessageHandler m_OnBinaryMessageRecv;

		public bool isDone;

		public bool connected;

		private Thread outgoingWorkerThread;

		private Thread incomingWorkerThread;

		private ActiveConnection connection;

		private List<string> incomingText;

		private List<byte[]> incomingBinary;

		private List<OutgoingMessage> outgoing;

		private bool hasContinuousFrame;

		private OpCode continuousFrameOpCode;

		private List<byte> continuousFrameData;

		private bool receivedClosingHandshake;

		private CloseEventCode closeEventCode;

		private string closeEventReason;

		private bool closing;

		public event OnTextMessageHandler OnTextMessageRecv;

		public event OnBinaryMessageHandler OnBinaryMessageRecv;

		private void OnTextMessage(string msg)
		{
		}

		private void OnBinaryMessage(byte[] msg)
		{
		}

		[IteratorStateMachine(typeof(_003CDispatcher_003Ed__44))]
		public IEnumerator Dispatcher()
		{
			return null;
		}

		public void Connect(string uri)
		{
		}

		public Coroutine Wait()
		{
			return null;
		}

		[IteratorStateMachine(typeof(_003C_Wait_003Ed__47))]
		private IEnumerator _Wait()
		{
			return null;
		}

		public void Connect(Uri uri)
		{
		}

		public void Send(string data)
		{
		}

		public void Send(byte[] data)
		{
		}

		public void Close(CloseEventCode code, string reason)
		{
		}

		private void OutgoingWorker()
		{
		}

		private void IncomingWorker()
		{
		}

		private bool ProcessBuffer(List<byte> buffer)
		{
			return false;
		}

		private bool ProcessFrame(List<byte> buffer)
		{
			return false;
		}

		private ParseFrameResult ParseFrame(List<byte> buffer, out FrameData frame)
		{
			frame = null;
			return ParseFrameResult.FrameIncomplete;
		}

		private byte[] BuildFrame(OpCode opCode, byte[] data)
		{
			return null;
		}

		private void RemoveProcessed(List<byte> buffer, int length)
		{
		}

		private void StartClosingHandshake(CloseEventCode code, string reason)
		{
		}

		private string WebSocketKey()
		{
			return null;
		}
	}
}
