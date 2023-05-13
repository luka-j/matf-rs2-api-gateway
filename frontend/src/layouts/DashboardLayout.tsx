import { Navigate, Outlet } from "react-router-dom";

import { useUserStore } from "@/stores/userStore";

const DashboardLayout = () => {
  const { currentUser, logoutUser } = useUserStore((state) => state);

  if (!currentUser) return <Navigate to="/login" />;

  return (
    <div>
      DashboardLayout
      <Outlet />
      <button
        onClick={logoutUser}
        className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded"
      >
        Logout
      </button>
    </div>
  );
};

export default DashboardLayout;
