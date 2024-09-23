const net = require('net');
const readline = require('readline');

class User {
    constructor(alias) {
        this.alias = alias;
    }
}

class Message {
    constructor(msg, userFrom) {
        this.msg = msg;
        this.userFrom = userFrom;
    }
}

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

let client = new net.Socket();

console.clear();

rl.question('Ingrese el Host IP: ', (hostIP) => {
    rl.question('Ingrese el puerto: ', (port) => {
        client.connect(parseInt(port), hostIP, () => {
            console.log('Conectado al servidor.');

            rl.question('Ingrese alias: ', (alias) => {
                const user = new User(alias);
                sendObject(user);

                serverConnection();

                sendMessageLoop(alias);
            });
        });
    });
});

// Función para enviar objetos
function sendObject(obj) {
    client.write(JSON.stringify(obj));
}

// Función para recibir mensajes del servidor
function serverConnection() {
    client.on('data', (data) => {
        try {
            const receivedMessage = JSON.parse(data.toString());
            console.log(`>>> ${receivedMessage.msg}`);
        } catch (err) {
            console.error("Error al deserializar el mensaje: ", err);
        }
    });
}

// Función para enviar mensajes continuamente
function sendMessageLoop(alias) {
    rl.on('line', (input) => {
        const message = new Message(input, alias);
        client.write(JSON.stringify(message));

        // Vuelve a mostrar el prompt
        rl.prompt();
    });
}

// Manejar cierre de conexión
client.on('close', () => {
    console.log('Conexión cerrada.');
    rl.close();
});
