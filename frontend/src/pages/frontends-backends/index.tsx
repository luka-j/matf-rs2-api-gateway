import { Loader2, PlusCircle, RefreshCcw, Trash } from "lucide-react";
import { useLocation, useNavigate } from "react-router-dom";

import { FrontendsBackendsConfig } from "@/types/api-configs";
import { cn } from "@/utils/style-utils";
import useGetFrontendsBackends from "@/hooks/use-get-frontends-backends";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";
import PageLoader from "@/components/page-loader";

import useDeleteFrontendBackend from "./hooks/use-delete-frontend-backend";
import useDeleteRp from "./hooks/use-delete-rp";
import useUpdateConfigs from "./hooks/use-update-configs";

const Frontends = () => {
  const location = useLocation();
  const frontOrBack = location.pathname.includes("frontend") ? "frontend" : "backend";

  const navigate = useNavigate();

  const { data: frontendsBackends, isLoading: isLoadingFrontends } =
    useGetFrontendsBackends(frontOrBack);

  const { mutate: deleteRp, isLoading: isDeletingRp } = useDeleteRp();

  const { mutate: deleteFrontendBackend, isLoading } = useDeleteFrontendBackend(frontOrBack);

  const { mutate: updateConfigs, isLoading: isUpdating } = useUpdateConfigs();

  if (isLoadingFrontends) return <PageLoader />;

  if (!frontendsBackends) return <div>Something went wrong</div>;

  const handleDeleteFrontBackRp = (frontendBackend: FrontendsBackendsConfig[number]) => {
    deleteFrontendBackend({ configType: frontOrBack, ...frontendBackend });
    deleteRp(frontendBackend);
  };

  return (
    <div className="container mt-8 flex flex-col gap-8">
      <div className="flex items-center justify-between">
        <Typography variant="h1">
          {frontOrBack === "frontend" ? "Frontends" : "Backends"}
        </Typography>
        <div className="flex gap-8">
          <Button variant="ghost" size="icon" disabled={isUpdating} onClick={() => updateConfigs()}>
            {isUpdating ? <Loader2 className="animate-spin" size={60} /> : <RefreshCcw size={60} />}
          </Button>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => navigate("/dashboard/spec", { state: { configType: frontOrBack } })}
          >
            <PlusCircle size={60} />
          </Button>
        </div>
      </div>

      <Card>
        <CardContent className="flex flex-wrap justify-center gap-5 p-5 md:justify-between md:gap-10 md:p-10">
          {frontendsBackends.map((frontendBackend) => (
            <Card
              key={frontendBackend.apiName + frontendBackend.apiVersion + frontendBackend.basePath}
              className="w-96"
            >
              <CardHeader className="flex flex-row items-center justify-between">
                <CardTitle>{frontendBackend.apiName}</CardTitle>
                <Button
                  variant="destructive"
                  size="icon"
                  onClick={() => handleDeleteFrontBackRp(frontendBackend)}
                  disabled={isLoading || isDeletingRp}
                >
                  {isLoading || isDeletingRp ? <Loader2 className="animate-spin" /> : <Trash />}
                </Button>
              </CardHeader>

              <CardContent className="flex flex-col justify-center gap-4">
                <Typography variant="large" className="text-center">
                  {frontendBackend.basePath}
                </Typography>
              </CardContent>

              <CardFooter
                className={cn(
                  "flex flex-row items-center",
                  frontOrBack === "frontend" ? "justify-between" : "justify-center",
                )}
              >
                <Button
                  size="lg"
                  variant="secondary"
                  onClick={() =>
                    navigate("/dashboard/spec", {
                      state: { configType: frontOrBack, ...frontendBackend },
                    })
                  }
                >
                  View spec
                </Button>
                {frontOrBack === "frontend" && (
                  <Button
                    size="lg"
                    variant="secondary"
                    onClick={() =>
                      navigate("/dashboard/middleware", {
                        state: frontendBackend,
                      })
                    }
                  >
                    View middleware
                  </Button>
                )}
              </CardFooter>
            </Card>
          ))}
        </CardContent>
      </Card>
    </div>
  );
};

export default Frontends;
