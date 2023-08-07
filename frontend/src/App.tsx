import { Suspense } from "react";
import { RouterProvider } from "react-router-dom";

import PageLoader from "./components/page-loader";
import { router } from "./configs/routes-config";
import { ThemeProvider } from "./lib/theme-provider";

function App() {
  return (
    <>
      <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
        <Suspense fallback={<PageLoader />}>
          <RouterProvider router={router} />
        </Suspense>
      </ThemeProvider>
    </>
  );
}

export default App;
