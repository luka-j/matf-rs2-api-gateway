import { lazy } from "react";
import { createRoutesFromElements, Navigate, Route } from "react-router-dom";
import { createBrowserRouter } from "react-router-dom";

const DashboardLayout = lazy(() => import("../layouts/DashboardLayout"));

const Login = lazy(() => import("../pages/Login"));
const Callback = lazy(() => import("../pages/Callback"));

const Overview = lazy(() => import("../pages/Overview"));
const Datasources = lazy(() => import("../pages/Datasources"));
const Frontends = lazy(() => import("../pages/Frontends"));
const Backends = lazy(() => import("../pages/Backends"));
const Editor = lazy(() => import("../pages/Editor"));

export const router = createBrowserRouter(
  createRoutesFromElements(
    <>
      <Route path="/" element={<Navigate to="/login" />} />
      <Route path="/login" element={<Login />} />
      <Route path="/callback" element={<Callback />} />

      <Route path="/dashboard" element={<DashboardLayout />}>
        <Route path="overview" element={<Overview />} />
        <Route path="datasources" element={<Datasources />} />
        <Route path="frontends" element={<Frontends />} />
        <Route path="backends" element={<Backends />} />
        <Route path="editor" element={<Editor />} />
        <Route path="*" element={<Navigate to="/overview" />} />
      </Route>

      <Route path="*" element={<Navigate to="/login" />} />
    </>,
  ),
);
