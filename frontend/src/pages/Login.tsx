import { Navigate } from "react-router-dom";

import { useUserStore } from "@/stores/userStore";

const Login = () => {
  const { loginUser, currentUser } = useUserStore((state) => state);

  if (currentUser) return <Navigate to="/dashboard" />;

  return (
    <section className="bg-gray-900 h-screen">
      <div className="flex flex-col items-center justify-center px-6 py-8 mx-auto md:h-screen lg:py-0 h-screen">
        <p className="flex items-center mb-6 text-2xl font-semibold ">Welcome to API Gateway</p>
        <div className="w-full rounded-lg shadow border md:mt-0 sm:max-w-md xl:p-0 bg-gray-800 border-gray-700">
          <div className="p-6 space-y-4 md:space-y-6 sm:p-8">
            <h1 className="text-xl font-bold leading-tight tracking-tight text-center  md:text-2xl ">
              Sign in to your account
            </h1>
            <form className="space-y-4 md:space-y-6" action="#">
              <button
                type="submit"
                className="w-full  bg-indigo-500 border-0 py-2 px-6 focus:outline-none hover:bg-indigo-600 rounded text-lg"
                onClick={loginUser}
              >
                Sign in
              </button>
            </form>
          </div>
        </div>
      </div>
    </section>
  );
};

export default Login;
