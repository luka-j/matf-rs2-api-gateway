import { Navigate, Outlet } from "react-router-dom";

import { useUserStore } from "@/stores/userStore";

import Navbar from "./components/Navbar";

const DashboardLayout = () => {
  const { currentUser } = useUserStore((state) => state);

  if (!currentUser) return <Navigate to="/login" />;

  return (
    <div className="bg-gray-900 min-h-screen">
      <Navbar />
      <Outlet />
    </div>
  );
};

export default DashboardLayout;
