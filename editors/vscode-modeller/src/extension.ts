import * as vscode from 'vscode';

const ICON_THEME_ID = 'modeller-icons';
const PROMPT_SHOWN_KEY = 'modeller.iconThemePromptShown';

export function activate(context: vscode.ExtensionContext) {
    // Check if we've already prompted the user
    const promptShown = context.globalState.get<boolean>(PROMPT_SHOWN_KEY, false);
    
    if (!promptShown) {
        // Check if our icon theme is already active
        const currentTheme = vscode.workspace.getConfiguration('workbench').get<string>('iconTheme');
        
        if (currentTheme !== ICON_THEME_ID) {
            // Prompt the user
            vscode.window.showInformationMessage(
                'Modeller DSL: Would you like to enable custom file icons for Modeller files?',
                'Yes, enable icons',
                'No thanks',
                'Remind me later'
            ).then(selection => {
                if (selection === 'Yes, enable icons') {
                    // Set the icon theme
                    vscode.workspace.getConfiguration('workbench').update(
                        'iconTheme',
                        ICON_THEME_ID,
                        vscode.ConfigurationTarget.Global
                    );
                    vscode.window.showInformationMessage('Modeller DSL Icons enabled!');
                    context.globalState.update(PROMPT_SHOWN_KEY, true);
                } else if (selection === 'No thanks') {
                    // Don't ask again
                    context.globalState.update(PROMPT_SHOWN_KEY, true);
                }
                // 'Remind me later' or dismissed - don't update the flag, will ask again next time
            });
        } else {
            // Already using our theme, no need to prompt
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        }
    }
}

export function deactivate() {}

