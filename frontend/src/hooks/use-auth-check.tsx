import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";

import checkJWT from "@/utils/check-jwt";

const useAuthCheck = () => {
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (location.pathname === "/login") {
      if (checkJWT()) navigate("/dashboard/overview");
    } else {
      if (!checkJWT()) {
        localStorage.setItem("redirectURL", location.pathname);
        navigate("/login");
      }
    }
  }, [location.pathname, navigate]);

  return;
};

export default useAuthCheck;
