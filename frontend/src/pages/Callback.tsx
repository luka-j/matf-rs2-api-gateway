import { useEffect } from "react";
import { useUserStore } from "@/stores/user-store";
import { useNavigate } from "react-router-dom";

import PageLoader from "@/components/page-loader";

const Callback = () => {
  const { currentUser, loginAndSetUser } = useUserStore((state) => state);

  const navigate = useNavigate();

  useEffect(() => {
    if (!currentUser) {
      loginAndSetUser().then(() => navigate("/dashboard/overview"));
    }
  }, [currentUser, loginAndSetUser, navigate]);

  return <PageLoader />;
};

export default Callback;
