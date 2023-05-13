import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

import Loader from "@/components/Loader";
import { useUserStore } from "@/stores/userStore";

const Callback = () => {
  const { currentUser, loginAndSetUser } = useUserStore((state) => state);

  const navigate = useNavigate();

  useEffect(() => {
    if (!currentUser) {
      loginAndSetUser().then(() => navigate("/dashboard"));
    }
  }, [currentUser, loginAndSetUser, navigate]);

  return (
    <div className="w-screen h-screen flex items-center justify-center">
      <Loader className="w-24 h-24" />
    </div>
  );
};

export default Callback;
