import { Navigate } from "react-router-dom";

import { useUserStore } from "@/stores/userStore";

const Login = () => {
  const { loginUser, currentUser } = useUserStore((state) => state);

  if (currentUser) return <Navigate to="/dashboard" />;

  return (
    <div>
      Hello, please login here:
      <button className="px-4 py-2 bg-blue-500 text-white rounded" onClick={loginUser}>
        Login
      </button>
    </div>
  );
};

export default Login;
