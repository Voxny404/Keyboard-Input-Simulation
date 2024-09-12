const key = require("./keyboard/key");
const sleep = ms => new Promise(r => setTimeout(r, ms));
const sleepTimer = 1000

setTimeout(async () => {

    await sleep(sleepTimer);
    key.sendWord("Testing keys!");

    const testKeyList = ["up", "down", "left", "right", "enter", "space", "tab"];
    for (const testKey of testKeyList) {
        await sleep(sleepTimer);
        key.send(testKey);
    }

    key.send("enter");
    const list = "asdfghjklöäqwertzuiopüyxcvbnm1234567890ß´+#,.-!§$%&/()=?^@€-_:;°*/".split("")

    for (const testKey of list) {
        key.send(testKey);
    }

    await sleep(sleepTimer)
    
    key.sendWord(`
        "Lorem ipsum dolor sit amet, consectetur adipiscing elit, 
        sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. 
        Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut 
        aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in 
        voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint 
        occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit 
        anim id est laborum."`
    );


},2000)