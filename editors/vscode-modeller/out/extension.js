"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || (function () {
    var ownKeys = function(o) {
        ownKeys = Object.getOwnPropertyNames || function (o) {
            var ar = [];
            for (var k in o) if (Object.prototype.hasOwnProperty.call(o, k)) ar[ar.length] = k;
            return ar;
        };
        return ownKeys(o);
    };
    return function (mod) {
        if (mod && mod.__esModule) return mod;
        var result = {};
        if (mod != null) for (var k = ownKeys(mod), i = 0; i < k.length; i++) if (k[i] !== "default") __createBinding(result, mod, k[i]);
        __setModuleDefault(result, mod);
        return result;
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
exports.activate = activate;
exports.deactivate = deactivate;
const fs = __importStar(require("fs"));
const path = __importStar(require("path"));
const vscode = __importStar(require("vscode"));
const node_1 = require("vscode-languageclient/node");
const ICON_THEME_ID = 'modeller-icons';
const PROMPT_SHOWN_KEY = 'modeller.iconThemePromptShown';
const MODELLER_LANGUAGES = [
    'modeller-def', 'modeller-entity', 'modeller-key', 'modeller-enum',
    'modeller-flags', 'modeller-service', 'modeller-command', 'modeller-query',
    'modeller-value', 'modeller-shared', 'modeller-event', 'modeller-projection',
    'modeller-union'
];
let client;
async function activate(context) {
    promptForIconTheme(context);
    await startLanguageServer(context);
}
async function deactivate() {
    if (client) {
        await client.stop();
        client = undefined;
    }
}
// ── icon theme prompt ──────────────────────────────────────────────────────────
function promptForIconTheme(context) {
    const promptShown = context.globalState.get(PROMPT_SHOWN_KEY, false);
    if (promptShown)
        return;
    const currentTheme = vscode.workspace.getConfiguration('workbench').get('iconTheme');
    if (currentTheme === ICON_THEME_ID) {
        context.globalState.update(PROMPT_SHOWN_KEY, true);
        return;
    }
    vscode.window.showInformationMessage('Modeller DSL: Would you like to enable custom file icons for Modeller files?', 'Yes, enable icons', 'No thanks', 'Remind me later').then(selection => {
        if (selection === 'Yes, enable icons') {
            vscode.workspace.getConfiguration('workbench').update('iconTheme', ICON_THEME_ID, vscode.ConfigurationTarget.Global);
            vscode.window.showInformationMessage('Modeller DSL Icons enabled!');
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        }
        else if (selection === 'No thanks') {
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        }
    });
}
// ── language server ────────────────────────────────────────────────────────────
async function startLanguageServer(context) {
    const serverDll = context.asAbsolutePath(path.join('server', 'Modeller.LanguageServer.dll'));
    if (!fs.existsSync(serverDll)) {
        // Server not bundled — silently skip (dev scenario without a build)
        return;
    }
    const serverOptions = {
        run: {
            command: 'dotnet',
            args: [serverDll],
            transport: node_1.TransportKind.stdio
        },
        debug: {
            command: 'dotnet',
            args: [serverDll],
            transport: node_1.TransportKind.stdio
        }
    };
    const clientOptions = {
        documentSelector: MODELLER_LANGUAGES.map(lang => ({
            scheme: 'file',
            language: lang
        }))
    };
    client = new node_1.LanguageClient('modeller-language-server', 'Modeller Language Server', serverOptions, clientOptions);
    await client.start();
    context.subscriptions.push(client);
}
//# sourceMappingURL=extension.js.map