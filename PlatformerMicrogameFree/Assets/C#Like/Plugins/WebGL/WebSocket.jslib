var LibraryWebSockets = {
$webSocketInstances: [],

SocketCreate: function(url)
{
	var str = Pointer_stringify(url);
	var socket = {
		socket: new WebSocket(str),
		buffer: new Uint8Array(0),
		error: null,
		messages: [],
		messagesType: []
	}

	socket.socket.binaryType = 'arraybuffer';

	socket.socket.onmessage = function (e) {
		// Todo: handle other data types?
		if (e.data instanceof Blob)
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				socket.messages.push(array);
				socket.messagesType.push(true);
			});
			reader.readAsArrayBuffer(e.data);
		}
		else if (e.data instanceof ArrayBuffer)
		{
			var array = new Uint8Array(e.data);
			socket.messages.push(array);
			socket.messagesType.push(false);
		}
		else if(typeof e.data === "string") {
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				socket.messages.push(array);
				socket.messagesType.push(true);
			});
			var blob = new Blob([e.data]);
			reader.readAsArrayBuffer(blob);
		}
	};

	socket.socket.onclose = function (e) {
		if (e.code != 1000)
		{
			if (e.reason != null && e.reason.length > 0)
				socket.error = e.reason;
			else
			{
				switch (e.code)
				{
					case 1001:
						socket.error = "Endpoint going away.";
						break;
					case 1002:
						socket.error = "Protocol error.";
						break;
					case 1003:
						socket.error = "Unsupported message.";
						break;
					case 1005:
						socket.error = "No status.";
						break;
					case 1006:
						socket.error = "Abnormal disconnection.";
						break;
					case 1009:
						socket.error = "Data frame too large.";
						break;
					default:
						socket.error = "Error "+e.code;
				}
			}
		}
	}
	var instance = webSocketInstances.push(socket) - 1;
	return instance;
},

SocketState: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	return socket.socket.readyState;
},

SocketError: function (socketInstance, ptr)
{
 	var socket = webSocketInstances[socketInstance];
 	if (socket.error == null)
 		return 0;
	console.log(socket.error);
	var length = lengthBytesUTF8(socket.error) + 1;
	var buffer = _malloc(length);
	stringToUTF8(socket.error, buffer, length);
	try {
		HEAPU8.set(buffer, ptr);
	} finally {
		_free(buffer);
	}
	socket.error = null;
	return 1;
},

SocketSendBinary: function (socketInstance, ptr, length)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.send (HEAPU8.buffer.slice(ptr, ptr+length));
},

SocketSendText: function (socketInstance, message)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.send (UTF8ToString(message));
},

SocketRecvLength: function(socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return 0;
	return socket.messages[0].length;
},

SocketRecv: function (socketInstance, ptr, length)
{
	var socket = webSocketInstances[socketInstance];
	if (socket.messages.length == 0)
		return false;
	if (socket.messages[0].length > length)
		return false;
	HEAPU8.set(socket.messages[0], ptr);
	socket.messages = socket.messages.slice(1);
	var ret = socket.messagesType[0];
	socket.messagesType = socket.messagesType.slice(1);
	return ret;
},

SocketClose: function (socketInstance)
{
	var socket = webSocketInstances[socketInstance];
	socket.socket.close();
}
};

autoAddDeps(LibraryWebSockets, '$webSocketInstances');
mergeInto(LibraryManager.library, LibraryWebSockets);
