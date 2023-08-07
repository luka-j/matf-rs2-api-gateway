import { useUserStore } from "@/stores/user-store";
import { Navigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

const Login = () => {
  const { loginUser, currentUser } = useUserStore((state) => state);

  if (currentUser) return <Navigate to="/dashboard/overview" />;

  return (
    <section className="flex h-full flex-col items-center justify-center gap-4 overflow-y-hidden">
      <Typography variant="h2">Welcome to API Gateway</Typography>
      <Card>
        <CardHeader>
          <CardTitle>Please sign in to your account</CardTitle>
        </CardHeader>
        <CardContent className="flex w-full">
          <Button onClick={loginUser} className="w-full">
            Sign in with Zitadel
          </Button>
        </CardContent>
      </Card>
    </section>
  );
};

export default Login;
