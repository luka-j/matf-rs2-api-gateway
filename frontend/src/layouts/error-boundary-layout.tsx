import { useEffect } from "react";
import { ErrorBoundary, FallbackProps } from "react-error-boundary";
import { Outlet, useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Typography } from "@/components/ui/typography";

export const setWithExpiry = (key: string, value: string, ttl: number) => {
  const item = {
    value: value,
    expiry: new Date().getTime() + ttl,
  };
  localStorage.setItem(key, JSON.stringify(item));
};

export const getWithExpiry = (key: string) => {
  const itemString = window.localStorage.getItem(key);
  if (!itemString) return null;

  const item = JSON.parse(itemString);
  const isExpired = new Date().getTime() > item.expiry;

  if (isExpired) {
    localStorage.removeItem(key);
    return null;
  }

  return item.value;
};

const ErrorFallback = ({ error, resetErrorBoundary }: FallbackProps) => {
  const navigate = useNavigate();

  useEffect(() => {
    if (error.message && error.message.includes("Failed to fetch dynamically imported module")) {
      if (!getWithExpiry("chunk_failed")) {
        setWithExpiry("chunk_failed", "true", 10000);
        navigate(0);
      }
    }
  }, [error, navigate]);

  return (
    <div className="flex h-screen flex-col items-center justify-center gap-2">
      <Typography variant="h2">
        An error occured in the application, please contact support with the following details:
      </Typography>
      <Typography variant="large">{error.message}</Typography>
      <div className="flex items-center gap-4">
        <Button
          onClick={() => {
            resetErrorBoundary();
            navigate("/");
          }}
        >
          Back Home
        </Button>
        <Typography variant="muted">or</Typography>
        <Button
          variant="secondary"
          onClick={() => {
            resetErrorBoundary();
          }}
        >
          Reload
        </Button>
      </div>
    </div>
  );
};

const ErrorBoundaryLayout = () => {
  return (
    <ErrorBoundary FallbackComponent={ErrorFallback}>
      <Outlet />
    </ErrorBoundary>
  );
};

export default ErrorBoundaryLayout;
