/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
    "./node_modules/@tremor/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        success: "#51DA88",
        error: "#DA5151",
        warning: "#F5AF45",
        info: "#51B1DA",
      },
    },
  },
  plugins: [],
};
