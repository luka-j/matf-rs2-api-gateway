import { Suspense } from "react";
import { QueryClientProvider } from "@tanstack/react-query";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { RouterProvider } from "react-router-dom";
import { ToastContainer } from "react-toastify";

import PageLoader from "./components/page-loader";
import { queryClient } from "./configs/react-query-config";
import { router } from "./configs/routes-config";
import { ThemeProvider } from "./lib/theme-provider";

import "react-toastify/dist/ReactToastify.css";

function App() {
  return (
    <>
      <QueryClientProvider client={queryClient}>
        <ThemeProvider defaultTheme="dark" storageKey="theme-mode">
          <Suspense fallback={<PageLoader />}>
            <RouterProvider router={router} />
          </Suspense>
          <ReactQueryDevtools initialIsOpen={false} />
          <ToastContainer position="bottom-right" autoClose={3000} />
        </ThemeProvider>
      </QueryClientProvider>
    </>
  );
}

export default App;
