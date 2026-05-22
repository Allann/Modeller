import * as fs from 'fs';
import * as path from 'path';
import * as vscode from 'vscode';
import {
    LanguageClient,
    LanguageClientOptions,
    ServerOptions,
    TransportKind
} from 'vscode-languageclient/node';

const MODELLER_LANGUAGES = [
    'modeller',
    'modeller-def', 'modeller-entity', 'modeller-key', 'modeller-enum',
    'modeller-flags', 'modeller-service', 'modeller-command', 'modeller-query',
    'modeller-value', 'modeller-shared', 'modeller-event', 'modeller-projection',
    'modeller-union'
];

let client: LanguageClient | undefined;

export async function activate(context: vscode.ExtensionContext): Promise<void> {
    await startLanguageServer(context);
}

export async function deactivate(): Promise<void> {
    if (client) {
        await client.stop();
        client = undefined;
    }
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
