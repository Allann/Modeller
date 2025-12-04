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
const vscode = __importStar(require("vscode"));
const ICON_THEME_ID = 'modeller-icons';
const PROMPT_SHOWN_KEY = 'modeller.iconThemePromptShown';
function activate(context) {
    // Check if we've already prompted the user
    const promptShown = context.globalState.get(PROMPT_SHOWN_KEY, false);
    if (!promptShown) {
        // Check if our icon theme is already active
        const currentTheme = vscode.workspace.getConfiguration('workbench').get('iconTheme');
        if (currentTheme !== ICON_THEME_ID) {
            // Prompt the user
            vscode.window.showInformationMessage('Modeller DSL: Would you like to enable custom file icons for Modeller files?', 'Yes, enable icons', 'No thanks', 'Remind me later').then(selection => {
                if (selection === 'Yes, enable icons') {
                    // Set the icon theme
                    vscode.workspace.getConfiguration('workbench').update('iconTheme', ICON_THEME_ID, vscode.ConfigurationTarget.Global);
                    vscode.window.showInformationMessage('Modeller DSL Icons enabled!');
                    context.globalState.update(PROMPT_SHOWN_KEY, true);
                }
                else if (selection === 'No thanks') {
                    // Don't ask again
                    context.globalState.update(PROMPT_SHOWN_KEY, true);
                }
                // 'Remind me later' or dismissed - don't update the flag, will ask again next time
            });
        }
        else {
            // Already using our theme, no need to prompt
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        }
    }
}
function deactivate() { }
//# sourceMappingURL=extension.js.map