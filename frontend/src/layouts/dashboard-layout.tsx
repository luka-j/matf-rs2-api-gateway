import { useUserStore } from "@/stores/user-store";
import { Navigate, Outlet } from "react-router-dom";

const DashboardLayout = () => {
  const { currentUser } = useUserStore((state) => state);

  if (!currentUser) return <Navigate to="/login" />;

  return (
    <div className="min-h-screen">
      <Outlet />
    </div>
  );
};

export default DashboardLayout;
