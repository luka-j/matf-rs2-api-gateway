import { FrontendsBackendsConfig } from "@/types/api-configs";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";

interface IConfigOverviewCardProps {
  title: string;
  viewAllURL: string;
  frontBackList: FrontendsBackendsConfig;
}

const ConfigOverviewCard = ({ title, viewAllURL, frontBackList }: IConfigOverviewCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex justify-between">
          {title}: {frontBackList.configs.length}/{frontBackList.configs.length} up
          <Button asChild variant="outline" className="-mt-1">
            <a href={viewAllURL}>View all</a>
          </Button>
        </CardTitle>
        <CardDescription>Here you can preview and edit your {title.toLowerCase()}.</CardDescription>
      </CardHeader>
      <CardContent className="grid gap-6">
        {frontBackList.configs.slice(0, 3).map((frontBack) => (
          <div key={frontBack.basePath} className="flex items-center justify-between space-x-4">
            <div className="flex items-center space-x-4">
              {/* {frontBack.status === "up" && <CheckIcon color="#10B981" />} */}
              {/* {frontBack.status === "down" && <X color="#EF4444" />} */}
              {/* {frontBack.status === "warn" && <AlertTriangle color="#FBBF24" />} */}
              <div>
                <p className="text-sm font-medium leading-none">{frontBack.apiName}</p>
                <p className="text-sm text-muted-foreground">Version {frontBack.apiVersion}</p>
              </div>
            </div>
            <Button variant="ghost">View spec</Button>
          </div>
        ))}
      </CardContent>
    </Card>
  );
};

export default ConfigOverviewCard;
