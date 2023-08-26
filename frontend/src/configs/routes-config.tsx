import { lazy } from "react";
import RootLayout from "@/layouts/root-layout";
import { createBrowserRouter, createRoutesFromElements, Navigate, Route } from "react-router-dom";

const ErrorBoundaryLayout = lazy(() => import("../layouts/error-boundary-layout"));

const DashboardLayout = lazy(() => import("../layouts/dashboard-layout"));

const Login = lazy(() => import("../pages/login"));
const Callback = lazy(() => import("../pages/callback"));

const Overview = lazy(() => import("../pages/overview"));
const Datasources = lazy(() => import("../pages/datasources"));
const FrontendsBackends = lazy(() => import("../pages/frontends-backends"));
const Spec = lazy(() => import("../pages/spec"));
const Middleware = lazy(() => import("../pages/middleware"));

export const router = createBrowserRouter(
  createRoutesFromElements(
    <>
      <Route element={<ErrorBoundaryLayout />}>
        <Route element={<RootLayout />}>
          <Route path="/" element={<Navigate to="/login" />} />
          <Route path="/login" element={<Login />} />
          <Route path="/callback" element={<Callback />} />

          <Route path="/dashboard" element={<DashboardLayout />}>
            <Route path="overview" element={<Overview />} />
            <Route path="datasources" element={<Datasources />} />
            <Route path="frontends" element={<FrontendsBackends key="frontends" />} />
            <Route path="backends" element={<FrontendsBackends key="backends" />} />
            <Route path="middleware" element={<Middleware />} />
            <Route path="spec" element={<Spec />} />
            <Route path="*" element={<Navigate to="/" />} />
          </Route>

          <Route path="*" element={<Navigate to="/login" />} />
        </Route>
      </Route>
    </>,
  ),
);
