import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import {
    LanguageClient,
    LanguageClientOptions,
    ServerOptions,
    TransportKind
} from 'vscode-languageclient/node';

const ICON_THEME_ID    = 'modeller-icons';
const PROMPT_SHOWN_KEY = 'modeller.iconThemePromptShown';

const MODELLER_LANGUAGES = [
    'modeller-def', 'modeller-entity', 'modeller-key', 'modeller-enum',
    'modeller-flags', 'modeller-service', 'modeller-command', 'modeller-query',
    'modeller-value', 'modeller-shared', 'modeller-event', 'modeller-projection',
    'modeller-union'
];

let client: LanguageClient | undefined;

export async function activate(context: vscode.ExtensionContext): Promise<void> {
    promptForIconTheme(context);
    await startLanguageServer(context);
}

export async function deactivate(): Promise<void> {
    if (client) {
        await client.stop();
        client = undefined;
    }
}

// ── icon theme prompt ──────────────────────────────────────────────────────────

function promptForIconTheme(context: vscode.ExtensionContext): void {
    const promptShown = context.globalState.get<boolean>(PROMPT_SHOWN_KEY, false);
    if (promptShown) return;

    const currentTheme = vscode.workspace.getConfiguration('workbench').get<string>('iconTheme');
    if (currentTheme === ICON_THEME_ID) {
        context.globalState.update(PROMPT_SHOWN_KEY, true);
        return;
    }

    vscode.window.showInformationMessage(
        'Modeller DSL: Would you like to enable custom file icons for Modeller files?',
        'Yes, enable icons',
        'No thanks',
        'Remind me later'
    ).then(selection => {
        if (selection === 'Yes, enable icons') {
            vscode.workspace.getConfiguration('workbench').update(
                'iconTheme',
                ICON_THEME_ID,
                vscode.ConfigurationTarget.Global
            );
            vscode.window.showInformationMessage('Modeller DSL Icons enabled!');
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        } else if (selection === 'No thanks') {
            context.globalState.update(PROMPT_SHOWN_KEY, true);
        }
    });
}

// ── language server ────────────────────────────────────────────────────────────

async function startLanguageServer(context: vscode.ExtensionContext): Promise<void> {
    const serverDll = context.asAbsolutePath(
        path.join('server', 'Modeller.LanguageServer.dll')
    );

    if (!fs.existsSync(serverDll)) {
        // Server not bundled — silently skip (dev scenario without a build)
        return;
    }

    const serverOptions: ServerOptions = {
        run: {
            command: 'dotnet',
            args: [serverDll],
            transport: TransportKind.stdio
        },
        debug: {
            command: 'dotnet',
            args: [serverDll],
            transport: TransportKind.stdio
        }
    };

    const clientOptions: LanguageClientOptions = {
        documentSelector: MODELLER_LANGUAGES.map(lang => ({
            scheme: 'file',
            language: lang
        }))
    };

    client = new LanguageClient(
        'modeller-language-server',
        'Modeller Language Server',
        serverOptions,
        clientOptions
    );

    await client.start();
    context.subscriptions.push(client);
}
