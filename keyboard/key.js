const os = require("os");
const { exec } = require('node:child_process');
const path = require('path');
const fs = require('fs');

class Key {
    constructor() {
        this.os = os.type().toLocaleLowerCase();
        this.windowsInputSwitch = false;
        this.sleep = ms => new Promise(r => setTimeout(r, ms));
        this.sleepTimer = 1000
    }

    send(key) {
        if (this.os !== "windows_nt") return console.log("KEY: ERROR = Only Windows is supported!!");
        this.#execute(key);
    }

    async sendWord(sentence) {
        const words = sentence.split(" ")
       
        const chars = []
        words.forEach((word) => {
            let isBeginingOfSentence = false
            word.split("").forEach( ch => {
                if (ch == "\n") {
                    chars.push("enter");
                    isBeginingOfSentence = true;
                }
                else chars.push(ch)
            })
            
            if (!isBeginingOfSentence) chars.push("space"); 
            isBeginingOfSentence = false;
        });

        for (const char of chars) {
            await this.#execute(char);
        }
       
    }

    #isNotExistingFile (path) {
        let isExisting = false
        fs.stat(path, (err) => err?.code === 'ENOENT' ? isExisting = false : isExisting = true);
        return isExisting
    }

    #execute(key) {
      
        const executablePath = path.join(__dirname, 'keyboard.exe');
        const language = "german"
        let command = `cmd /c "${executablePath} ${language} ${key}"`;

        // switches to legacy key input when file is not existing
        this.windowsInputSwitch = this.#isNotExistingFile(executablePath)

        // uses wshell instead of the keyboard.exe
        if (this.windowsInputSwitch) command = `powershell -c "$wshell = New-Object -ComObject wscript.shell; $wshell.SendKeys('{${key}}')`

        return new Promise(res => {
            exec(command, (err, stdout, stderr) => {
                if (err) return console.error(err);
                
                stdout ? console.log(stdout): null
                stderr ? console.log(stderr): null
                res();
            });
        })
    }
}

const singelton = new Key();
module.exports = singelton;