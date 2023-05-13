## Installation

1. Clone the repository: `git clone https://github.com/luka-j/matf-rs2-api-gateway.git`
2. Move into the frontend project directory: `cd frontend`
3. Open the project folder with VSCode (or your preferred editor) and open the integrated terminal
4. Install (or update) Node and NPM:
   - On windows, download and install the latest version of Node from [here](https://nodejs.org/en/download/).
   - On linux, run the following commands:
     - Install curl: `sudo apt install curl`
     - Install nvm: `curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.35.3/install.sh | bash`
     - Install node: `nvm install node`
     - Install npm: `nvm install-latest-npm`
5. Install **pnpm**: `npm i -g pnpm`
6. Install dependencies: `pnpm i`

## Development

To start the development server, run `pnpm run dev`. This will start the development server on port 3000. The server will automatically reload when you make changes to the code.

It's recommended to use [VSCode](https://code.visualstudio.com/) as your editor since it's configured to use the same settings as the project inside `.vscode/settings.json`. It's also recommended to use [ESLint](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint), [Prettier](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode) and [Prettier ESLint](https://marketplace.visualstudio.com/items?itemName=rvest.vs-code-prettier-eslint) extensions for VSCode for automatic code formatting and linting on file save. You don't need any additional setup since the extensions are configured to use the same settings as the ones used by the project inside `.vscode/settings.json`.

**NOTE:** I have setup alias imports for the `src` folder which makes imports shorter and easier to read. This means that you can import files from these folders using the `@` alias. For example, if you are in a deeply nested folder and you want to import component named `Component` from the `components` folder, you can do it like this: `import Component from '@/components/Component'`. This is the same as doing `import Component from '../../../components/Component'`.
