import { System } from "@/mock/overview";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Typography } from "@/components/ui/typography";

interface ISystemOverviewCardProps {
  title: string;
  description: string;
  numInstances: number;
  viewAllURL: string;
  seeMetricsURL: string;
  systemOverviewList: System[];
}

const SystemOverviewCard = ({
  title,
  description,
  numInstances,
  viewAllURL,
  seeMetricsURL,
  systemOverviewList,
}: ISystemOverviewCardProps) => {
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex justify-between">
          {title}: {numInstances} instances
          <Button asChild variant="outline" className="-mt-1">
            <a href={viewAllURL}>View all</a>
          </Button>
        </CardTitle>
        <CardDescription>{description}</CardDescription>
      </CardHeader>
      <CardContent className="grid gap-6">
        {systemOverviewList.slice(0, 3).map((systemOverview) => (
          <div key={systemOverview.name} className="flex">
            <Typography variant="large">
              {systemOverview.name}{" "}
              <Typography as="span" variant="small">
                last config update at{" "}
              </Typography>
              {systemOverview.lastConfigUpdate}
            </Typography>
          </div>
        ))}
      </CardContent>
      <CardFooter className="flex justify-end">
        <Button variant="ghost">
          <a href={seeMetricsURL}>See metrics...</a>
        </Button>
      </CardFooter>
    </Card>
  );
};

export default SystemOverviewCard;
